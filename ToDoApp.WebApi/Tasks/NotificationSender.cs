using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using ToDoApp.Contract;
using ToDoApp.Core.Persistence;
using ToDoApp.Core.Service.AppService;
using ToDoApp.Entity.Model;
using ToDoApp.WebApi.Utility;

namespace ToDoApp.WebApi.Tasks
{
    public class NotificationSender
    {
        private readonly ToDoListService _toDoListService;
        private readonly TaskService _taskService;

        public NotificationSender()
        {
            var unitOfWork = IoCUtility.Resolve<IUnitOfWork<DbContext>>("isolatedUnitOfWork");
            var toDoListRepository = IoCUtility.Resolve<IToDoListRepository>("isolatedToDoListRepository");
            var taskRepository = IoCUtility.Resolve<ITaskRepository>("isolatedTaskRepository");

            _toDoListService = new ToDoListService(toDoListRepository, unitOfWork);
            _taskService = new TaskService(taskRepository, unitOfWork);
        }

        public void Execute()
        {
            IList<ToDoList> toDoLists;
            IList<Task> tasks;
            List<NotificationItemContract> notificationItems = GetAvailableItemsFromDb(out toDoLists, out tasks);
            SendNotification(notificationItems);
            UpdateDatabase(toDoLists, tasks);
        }

        private void UpdateDatabase(IList<ToDoList> toDoLists, IList<Task> tasks)
        {
            if (toDoLists.Any())
            {
                foreach (var toDoList in toDoLists)
                {
                    toDoList.IsNotificationSend = true;
                    _toDoListService.Save(toDoList);
                }
            }

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    task.IsNotificationSend = true;
                    _taskService.Save(task);
                }
            }
        }

        private void SendNotification(List<NotificationItemContract> notificationItems)
        {
            foreach (var notificationItem in notificationItems)
            {
                string bodyMessage = string.Format("Hi {0}, <br/>This notification for: {1}", notificationItem.FullName, notificationItem.Title);
                SendMail(notificationItem.Email, bodyMessage);
            }
        }

        private void SendMail(string mailTo, string bodyMessage)
        {
            MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["NotificationFromMail"], mailTo);
            SmtpClient client = new SmtpClient
            {
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["NotificationSmtpPort"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = ConfigurationManager.AppSettings["NotificationSmtpHost"],
                Credentials =
                    new NetworkCredential(
                        ConfigurationManager.AppSettings["NotificationSmtpUsername"],
                        ConfigurationManager.AppSettings["NotificationSmtpPassword"]
                        )
            };
            mail.Subject = ConfigurationManager.AppSettings["NotificationMailSubject"];
            mail.IsBodyHtml = true;
            mail.Body = bodyMessage;
            client.Send(mail);
        }

        private List<NotificationItemContract> GetAvailableItemsFromDb(out IList<ToDoList> toDoLists, out IList<Task> tasks)
        {
            List<NotificationItemContract> notificationItems = new List<NotificationItemContract>();

            toDoLists = _toDoListService.GetNotificationNotSendItems();
            foreach (var item in toDoLists)
            {
                notificationItems.Add(new NotificationItemContract
                {
                    Email = item.User.Email,
                    Title = item.Title,
                    FullName = item.User.Name + " " + item.User.Surname
                });
            }

            tasks = _taskService.GetNotificationNotSendItems();
            foreach (var item in tasks)
            {
                notificationItems.Add(new NotificationItemContract
                {
                    Email = item.ToDoList.User.Email,
                    Title = item.Title,
                    FullName = item.ToDoList.User.Name + " " + item.ToDoList.User.Surname
                });
            }

            return notificationItems;
        }
    }
}
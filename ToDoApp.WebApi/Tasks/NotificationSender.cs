using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
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
            SendMail();
            IList<ToDoList> toDoLists;
            IList<Task> tasks;
            List<NotificationItemContract> notificationItems = GetAvailableItemsFromDb(out toDoLists, out tasks);

        }

        private void SendNotification(List<NotificationItemContract> notificationItems)
        {
            
        }

        private void SendMail()
        {
            try
            {
                MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["NotificationFromMail"], "account@mumbstudio.com");
                SmtpClient client = new SmtpClient();
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["NotificationSmtpPort"]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["NotificationSmtpPort"], "c90fd8b5f6afa3");
                client.Host = "smtp.mailtrap.io";
                mail.Subject = "this is a test email.";
                mail.Body = "this is my test email body";
                client.Send(mail);
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
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
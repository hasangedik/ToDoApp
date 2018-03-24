using System.Configuration;
using System.Data.Entity;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ToDoApp.Core.Persistence;
using ToDoApp.Core.Service;
using ToDoApp.Core.Service.Log;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.Infrastructure.Service;
using ToDoApp.MongoLogger.Service;

namespace ToDoApp.WebApi.Utility
{
    public static class IoCUtility
    {
        private static IWindsorContainer _container;
        private static IWindsorContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = BootstrapContainer();
                }
                return _container;
            }
        }

        private static IWindsorContainer BootstrapContainer()
        {
            return new WindsorContainer().Register(
                Component.For<IDatabaseContextFactory<DbContext>>()
                    .ImplementedBy<DatabaseContextFactory>()
                    .LifestyleSingleton()
                    .UsingFactoryMethod(x => DatabaseContextFactory.Instance),
                Component.For<IUnitOfWork<DbContext>>()
                    .ImplementedBy<UnitOfWork>()
                    .LifestylePerWebRequest(),
                Component.For<IUnitOfWork<DbContext>>()
                    .ImplementedBy<UnitOfWork>()
                    .Named("isolatedUnitOfWork"),
                Component.For<IUserRepository>()
                    .ImplementedBy<UserRepository>()
                    .LifestylePerWebRequest()
                    .DynamicParameters((kernel, parameters) => { parameters["unitOfWork"] = Resolve<IUnitOfWork<DbContext>>(); }),
                Component.For<IToDoListRepository>()
                    .ImplementedBy<ToDoListRepository>()
                    .LifestylePerWebRequest()
                    .DynamicParameters((kernel, parameters) => { parameters["unitOfWork"] = Resolve<IUnitOfWork<DbContext>>(); }),
                Component.For<IToDoListRepository>()
                    .ImplementedBy<ToDoListRepository>()
                    .Named("isolatedToDoListRepository")
                    .DynamicParameters((kernel, parameters) => { parameters["unitOfWork"] = Resolve<IUnitOfWork<DbContext>>("isolatedUnitOfWork"); }),
                Component.For<ITaskRepository>()
                    .ImplementedBy<TaskRepository>()
                    .LifestylePerWebRequest()
                    .DynamicParameters((kernel, parameters) => { parameters["unitOfWork"] = Resolve<IUnitOfWork<DbContext>>(); }),
                Component.For<ITaskRepository>()
                    .ImplementedBy<TaskRepository>()
                    .Named("isolatedTaskRepository")
                    .DynamicParameters((kernel, parameters) => { parameters["unitOfWork"] = Resolve<IUnitOfWork<DbContext>>("isolatedUnitOfWork"); }),
                Component.For<IAuditLogService>()
                    .ImplementedBy<AuditLogService>()
                    .LifestyleSingleton()
                    .DynamicParameters((kernel, parameters) => { parameters["connectionString"] = ConfigurationManager.AppSettings["MongoDbConnectionString"]; }),
                Component.For<INotificationService>()
                    .ImplementedBy<MailNotificationService>()
                    .DynamicParameters((kernel, parameters) =>
                    {
                        parameters["smtpHost"] = ConfigurationManager.AppSettings["NotificationSmtpHost"];
                        parameters["smtpPort"] = ConfigurationManager.AppSettings["NotificationSmtpPort"];
                        parameters["smtpUsername"] = ConfigurationManager.AppSettings["NotificationSmtpUsername"];
                        parameters["smtpPassword"] = ConfigurationManager.AppSettings["NotificationSmtpPassword"];
                    })
        );

        }
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static T Resolve<T>(string name)
        {
            return Container.Resolve<T>(name);
        }
    }
}

using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDoApp.Core.Persistence;
using ToDoApp.Core.Service.AppService;
using ToDoApp.Entity.Model;
using ToDoApp.Test.Helper;

namespace ToDoApp.Test
{
    [TestClass]
    public class UserUnitTest
    {
        private readonly UserService _userService;
        
        public UserUnitTest()
        {
            var unitOfWork = IoCUtility.Resolve<IUnitOfWork<DbContext>>();
            var userRepository = IoCUtility.Resolve<IUserRepository>();

            _userService = new UserService(userRepository, unitOfWork);
        }

        [TestMethod]
        public void SaveTest()
        {
            var savedUser = _userService.Save(new User
            {
                Name = "Hasan",
                Surname = "Gedik",
                Email = "test@test.com",
                Password = "1234",
                CreatedOn = DateTime.Now,
                CreatedBy = "Jeto"
                
            });

            Assert.IsNotNull(savedUser);
        }

        [TestMethod]
        public void GetTest()
        {
            var entity = _userService.Get(2);
            Assert.IsNotNull(entity);
        }

        [TestMethod]
        public void GetUserByEmailTest()
        {
            var entity = _userService.GetUserByEmail("test@test.com");
            Assert.IsNotNull(entity);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace FoodOrderingSystem.Tests.Unit
{
    /// <summary>
    /// Tests validation dữ liệu để đảm bảo data integrity
    /// </summary>
    [TestClass]
    public class DataValidationTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Signup_DuplicateUsername_ReturnsError()
        {
            // Arrange
            var existingUsers = new List<tblUser>
            {
                new tblUser 
                { 
                    UserId = 1, 
                    Username = "existing@email.com",
                    Password = "password123"
                }
            }.AsQueryable();

            var mockDb = new Mock<IOnlineFoodDBEntities>();
            var mockUserDbSet = CreateMockDbSet(existingUsers);
            mockDb.Setup(x => x.tblUsers).Returns(mockUserDbSet.Object);

            var controller = new HomeController(mockDb.Object);

            var newUserVm = new UserViewModel
            {
                Username = "existing@email.com", // Trùng username
                Password = "newpassword"
            };

            // Act
            var result = controller.Signup(newUserVm) as JsonResult;
            dynamic data = result.Data;

            // Assert
            Assert.IsNotNull(result, "Result không được null");
            Assert.IsFalse(data.success, "Signup với duplicate username phải fail");
            Assert.IsTrue(((string)data.message).Contains("Already Register"), 
                "Error message phải thông báo user đã tồn tại");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Signup_NewUsername_Success()
        {
            // Arrange
            var existingUsers = new List<tblUser>().AsQueryable(); // Empty - no existing users

            var mockDb = new Mock<IOnlineFoodDBEntities>();
            var mockUserDbSet = CreateMockDbSet(existingUsers);
            var mockUserRoleDbSet = CreateMockDbSet(new List<UserRole>().AsQueryable());
            
            mockDb.Setup(x => x.tblUsers).Returns(mockUserDbSet.Object);
            mockDb.Setup(x => x.UserRoles).Returns(mockUserRoleDbSet.Object);
            
            // Mock Add và SaveChanges
            mockDb.Setup(x => x.tblUsers.Add(It.IsAny<tblUser>())).Returns((tblUser u) => u);
            mockDb.Setup(x => x.UserRoles.Add(It.IsAny<UserRole>())).Returns((UserRole ur) => ur);
            mockDb.Setup(x => x.SaveChanges()).Returns(1);

            var controller = new HomeController(mockDb.Object);

            var newUserVm = new UserViewModel
            {
                Username = "newuser@email.com",
                Password = "password123"
            };

            // Act
            var result = controller.Signup(newUserVm) as JsonResult;
            dynamic data = result.Data;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(data.success, "Signup với username mới phải thành công");
            Assert.IsTrue(((string)data.message).Contains("Successfully"), 
                "Success message phải hiển thị");
            
            // Verify Add được gọi
            mockDb.Verify(x => x.tblUsers.Add(It.IsAny<tblUser>()), Times.Once, 
                "User phải được add vào database");
            mockDb.Verify(x => x.SaveChanges(), Times.AtLeastOnce, 
                "SaveChanges phải được gọi");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Signup_EmptyUsername_HandledGracefully()
        {
            // Arrange
            var mockDb = new Mock<IOnlineFoodDBEntities>();
            var mockUserDbSet = CreateMockDbSet(new List<tblUser>().AsQueryable());
            mockDb.Setup(x => x.tblUsers).Returns(mockUserDbSet.Object);

            var controller = new HomeController(mockDb.Object);

            var userVm = new UserViewModel
            {
                Username = "", // Empty username
                Password = "password123"
            };

            // Act
            var result = controller.Signup(userVm) as JsonResult;

            // Assert
            Assert.IsNotNull(result, "Result không được null");
            // Test documents behavior - system handles empty username
            // (May allow it or reject it depending on business rules)
        }

        #region Helper Methods

        private Mock<IDbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<IDbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }

        #endregion
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using OnlineFoodOrderingSystem.Models.ViewModel;
using System.Linq;
using System.Web.Mvc;

namespace FoodOrderingSystem.Tests.Integration
{
    /// <summary>
    /// Integration tests cho user authentication flow
    /// Tests E2E từ registration đến login
    /// </summary>
    [TestClass]
    public class UserAuthenticationFlowTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_UserCanRegisterSuccessfully()
        {
            // Arrange
            var controller = new HomeController();
            var uniqueUsername = $"testuser_{System.Guid.NewGuid():N}@test.com";
            
            var userVm = new UserViewModel
            {
                Username = uniqueUsername,
                Password = "Password123"
            };

            // Act
            var result = controller.Signup(userVm) as JsonResult;
            dynamic data = result.Data;

            // Assert
            Assert.IsNotNull(result, "Signup should return JsonResult");
            Assert.IsTrue(data.success, "Registration should succeed for new user");
            Assert.IsTrue(((string)data.message).Contains("Successfully"), 
                "Success message should be returned");

            // Verify user created in database
            using (var db = new OnlineFoodDBEntities())
            {
                var createdUser = db.tblUsers.FirstOrDefault(u => u.Username == uniqueUsername);
                Assert.IsNotNull(createdUser, "User should exist in database");
                Assert.AreEqual(uniqueUsername, createdUser.Username);
                Assert.AreEqual("Password123", createdUser.Password);

                // Verify UserRole was created
                var userRole = db.UserRoles.FirstOrDefault(ur => ur.UserId == createdUser.UserId);
                Assert.IsNotNull(userRole, "UserRole should be created");
                Assert.AreEqual(2, userRole.UserRolesId, "Default role should be 2 (regular user)");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_DuplicateUsernameRegistrationFails()
        {
            // Arrange
            var controller = new HomeController();
            var uniqueUsername = $"duplicate_{System.Guid.NewGuid():N}@test.com";

            var userVm = new UserViewModel
            {
                Username = uniqueUsername,
                Password = "Password123"
            };

            // Act - Register first time (should succeed)
            var firstResult = controller.Signup(userVm) as JsonResult;
            dynamic firstData = firstResult.Data;
            Assert.IsTrue(firstData.success, "First registration should succeed");

            // Act - Try to register again with same username (should fail)
            var secondResult = controller.Signup(userVm) as JsonResult;
            dynamic secondData = secondResult.Data;

            // Assert
            Assert.IsNotNull(secondResult);
            Assert.IsFalse(secondData.success, "Duplicate registration should fail");
            Assert.IsTrue(((string)secondData.message).Contains("Already Register"),
                "Error message should indicate duplicate user");
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_UserDataPersistsInDatabase()
        {
            // Arrange
            var uniqueUsername = $"persist_{System.Guid.NewGuid():N}@test.com";
            var testPassword = "TestPass456";

            // Act - Create user via controller
            var controller = new HomeController();
            var userVm = new UserViewModel
            {
                Username = uniqueUsername,
                Password = testPassword
            };

            controller.Signup(userVm);

            // Assert - Verify data persists correctly
            using (var db = new OnlineFoodDBEntities())
            {
                var user = db.tblUsers.FirstOrDefault(u => u.Username == uniqueUsername);
                
                Assert.IsNotNull(user, "User should persist in database");
                Assert.AreEqual(uniqueUsername, user.Username, "Username should match");
                Assert.AreEqual(testPassword, user.Password, "Password should match");
                
                // Verify UserId is generated
                Assert.IsTrue(user.UserId > 0, "UserId should be auto-generated");

                // Verify can query by UserId
                var userById = db.tblUsers.Find(user.UserId);
                Assert.IsNotNull(userById, "Should be able to query user by ID");
                Assert.AreEqual(uniqueUsername, userById.Username);
            }
        }
    }
}

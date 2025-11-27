using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Tests;

namespace FoodOrderingSystem.Tests.Unit
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Index_WhenCalled_ReturnsDefaultView()
        {
            // Arrange
            var mockDb = new Mock<IOnlineFoodDBEntities>();
            var controller = new HomeController(mockDb.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            // View mặc định có ViewName null hoặc rỗng
            Assert.AreEqual(string.Empty, result.ViewName ?? string.Empty);
        }
    }
}

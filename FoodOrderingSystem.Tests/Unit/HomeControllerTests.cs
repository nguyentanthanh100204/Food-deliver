using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;

namespace FoodOrderingSystem.Tests.Unit
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Index_WhenCalled_ReturnsDefaultView()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            // View mặc định có ViewName null hoặc rỗng
            Assert.AreEqual(string.Empty, result.ViewName ?? string.Empty);
        }
    }
}

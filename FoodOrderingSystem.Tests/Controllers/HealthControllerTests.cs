using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;

namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class HealthControllerTests
    {
        [TestMethod]
        public void Index_Returns_OK_Content()
        {
            // Arrange
            var controller = new HealthController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Content);
        }
    }
}

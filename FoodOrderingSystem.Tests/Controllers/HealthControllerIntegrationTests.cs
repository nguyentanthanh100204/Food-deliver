using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;

namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class HealthControllerIntegrationTests
    {
        [TestMethod]
        public void Index_Returns_OK()
        {
            // Arrange
            var controller = new HealthController();

            // Act
            var result = controller.Index();

            // Assert: chấp nhận 1 trong 2 kiểu trả về phổ biến
            if (result is HttpStatusCodeResult code)
            {
                Assert.AreEqual(200, code.StatusCode, "Health should return HTTP 200.");
            }
            else if (result is ContentResult content)
            {
                Assert.IsTrue(
                    content.Content.Trim().ToUpperInvariant().Contains("OK"),
                    "Health should contain OK");
            }
            else
            {
                Assert.Fail($"Unexpected result type: {result?.GetType().FullName}");
            }
        }
    }
}

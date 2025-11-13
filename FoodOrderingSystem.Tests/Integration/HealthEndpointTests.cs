using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Tests;                 // để dùng TestCategories
using FoodOrderingSystem.Tests.Integration;     // để kế thừa IntegrationTestBase


namespace FoodOrderingSystem.Tests.Integration
{
    [TestClass]
    public class HealthEndpointTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Health_Index_Returns_OK()
        {
            // Arrange
            var controller = new HealthController();

            // Act
            var result = controller.Index();

            // Assert
            if (result is HttpStatusCodeResult code)
            {
                Assert.AreEqual(200, code.StatusCode, "Health should return HTTP 200.");
            }
            else if (result is ContentResult content)
            {
                Assert.IsTrue(
                    (content.Content ?? string.Empty).ToUpperInvariant().Contains("OK"),
                    "Health response should contain 'OK'.");
            }
            else
            {
                Assert.Fail($"Unexpected result type: {result?.GetType().FullName}");
            }
        }
    }
}

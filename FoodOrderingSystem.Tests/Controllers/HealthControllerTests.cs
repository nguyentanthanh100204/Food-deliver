using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Tests; // TestCategories


namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class HealthControllerTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]

        public void Index_Returns_OK()
        {
            var controller = new HealthController();
        
            ActionResult result = controller.Index();
        
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

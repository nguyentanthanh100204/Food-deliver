using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using FoodOrderingSystem.Tests; // TestCategories
using FoodOrderingSystem.Controllers;

namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class ShoppingCartController_CartSummary_Tests
    {
        private static ControllerContext BuildContext(Controller controller)
        {
            var http = new Mock<HttpContextBase>();
            http.SetupGet(x => x.Session).Returns(new FakeSession());

            return new ControllerContext(
                new RequestContext(http.Object, new RouteData()), controller);
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]

        public void CartSummary_ReturnsPartial_CartSummary()
        {
            // Arrange
            var controller = new ShoppingCartController();
            controller.ControllerContext = BuildContext(controller);

            // Act
            var result = controller.CartSummary() as PartialViewResult;

            // Assert
            Assert.IsNotNull(result, "Kết quả không phải PartialViewResult");
            // Controller trả về PartialView("CartSummary")
            Assert.AreEqual("CartSummary", result.ViewName, "Partial view name không đúng.");
            // Có thể kiểm tra ViewData["CartCount"] tồn tại (không null)
            Assert.IsTrue(controller.ViewData.ContainsKey("CartCount"));
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using System.Transactions;

using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using OnlineFoodOrderingSystem.Models.ViewModel;
using FoodOrderingSystem.Tests; // TestCategories


namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class ShoppingCartControllerTests
    {
        private static ControllerContext BuildContext(Controller controller, HttpSessionStateBase session = null)
        {
            var http = new Mock<HttpContextBase>();
            http.SetupGet(x => x.Session).Returns(session ?? new FakeSession());

            var ctx = new ControllerContext(
                new RequestContext(http.Object, new RouteData()), controller);

            return ctx;
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]

        public void ShoppingCartList_WhenCalled_ReturnsViewAndViewModel()
        {
            // Arrange
            var controller = new ShoppingCartController();
            controller.ControllerContext = BuildContext(controller);

            // Act
            var result = controller.ShoppingCartList() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            // Không đặt ViewName => MVC trả "" (view theo tên action)
            Assert.AreEqual(string.Empty, result.ViewName ?? string.Empty);

            // Model phải là ShoppingCartViewModel
            Assert.IsInstanceOfType(result.Model, typeof(ShoppingCartViewModel));

            var vm = (ShoppingCartViewModel)result.Model;
            // Không assert cụ thể số lượng, vì phụ thuộc DB/ShoppingCart
            // chỉ kiểm tra object tồn tại
            Assert.IsNotNull(vm.CartItems);
        }

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;

using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using OnlineFoodOrderingSystem.Models.ViewModel;
using FoodOrderingSystem.Tests; // TestCategories
using FoodOrderingSystem.Tests.TestHelpers; // hoặc đúng namespace của FakeSession.cs


namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class ShoppingCartControllerTests
    {
        private static ControllerContext BuildContext(Controller controller, HttpSessionStateBase session = null)
        {
            var http = new Mock<HttpContextBase>();
            http.SetupGet(x => x.Session).Returns(session ?? new FakeSession());

            // Mock User and Identity to prevent NullReferenceException in GetCartId
            var mockIdentity = new Mock<System.Security.Principal.IIdentity>();
            mockIdentity.Setup(x => x.Name).Returns(string.Empty);
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(false);

            var mockPrincipal = new Mock<System.Security.Principal.IPrincipal>();
            mockPrincipal.Setup(x => x.Identity).Returns(mockIdentity.Object);

            http.SetupGet(x => x.User).Returns(mockPrincipal.Object);

            var ctx = new ControllerContext(
                new RequestContext(http.Object, new RouteData()), controller);

            return ctx;
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void ShoppingCartList_WhenCalled_ReturnsViewAndViewModel()
        {
            // Arrange
            var mockDb = new Mock<IOnlineFoodDBEntities>();
            // Setup mock DbSet for tblCarts to avoid NullReferenceException
            var data = new List<tblCart>().AsQueryable();
            var mockSet = new Mock<System.Data.Entity.DbSet<tblCart>>();
            mockSet.As<IQueryable<tblCart>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<tblCart>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<tblCart>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<tblCart>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockDb.Setup(m => m.tblCarts).Returns(mockSet.Object);
            
            var controller = new ShoppingCartController(mockDb.Object);
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
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using FoodOrderingSystem.Tests;                 // TestCategories
using FoodOrderingSystem.Tests.Integration;     // IntegrationTestBase
using FoodOrderingSystem.Tests.TestHelpers; // hoặc đúng namespace của FakeSession.cs


using FoodOrderingSystem.Controllers;   // ShoppingCartController
using FoodOrderingSystem.Models;        // OnlineFoodDBEntities

namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class ShoppingCartController_AddToCart_Tests : IntegrationTestBase
    {
        /// <summary>
        /// Tạo ControllerContext tối thiểu để ShoppingCart.GetCart(HttpContext) hoạt động.
        /// </summary>
        private static ControllerContext BuildContext(Controller controller)
        {
            var http = new Mock<HttpContextBase>();
            http.SetupGet(x => x.Session).Returns(new FakeSession()); // session giả

            var ctx = new ControllerContext(
                new RequestContext(http.Object, new RouteData()), controller);

            return ctx;
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void AddToCart_WithExistingItem_RedirectsToShoppingCartList()
        {
            // Bọc giao dịch để mọi ghi phát sinh (nếu có) sẽ ROLLBACK sau test
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Arrange
                var controller = new ShoppingCartController();
                controller.ControllerContext = BuildContext(controller);

                int existingItemId;
                using (var db = new OnlineFoodDBEntities())
                {
                    var anyItem = db.tblItems.FirstOrDefault();
                    Assert.IsNotNull(anyItem, "DB chưa có dữ liệu tblItems để test AddToCart. Hãy đảm bảo script seed đã chạy.");
                    existingItemId = anyItem.ItemId;
                }

                // Act
                var result = controller.AddToCart(existingItemId) as RedirectToRouteResult;

                // Assert
                Assert.IsNotNull(result, "Kết quả không phải RedirectToRouteResult.");
                Assert.AreEqual("ShoppingCartList", result.RouteValues["action"]);
                // KHÔNG gọi scope.Complete(); -> rollback mọi thay đổi
            }
        }
    }
}

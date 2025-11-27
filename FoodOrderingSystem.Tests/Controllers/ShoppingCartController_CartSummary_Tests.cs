using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using FoodOrderingSystem.Tests; // TestCategories
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Tests.TestHelpers; 
using System.Data.Entity;
using System.Linq; 


namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class ShoppingCartController_CartSummary_Tests
    {
        private static Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }

        private static ControllerContext BuildContext(Controller controller)
        {
            var http = new Mock<HttpContextBase>();
            http.SetupGet(x => x.Session).Returns(new FakeSession());
            
            var user = new Mock<System.Security.Principal.IPrincipal>();
            var identity = new Mock<System.Security.Principal.IIdentity>();
            identity.Setup(i => i.Name).Returns("testuser");
            user.Setup(u => u.Identity).Returns(identity.Object);
            http.Setup(x => x.User).Returns(user.Object);

            return new ControllerContext(
                new RequestContext(http.Object, new RouteData()), controller);
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void CartSummary_ReturnsPartial_CartSummary()
        {
            // Arrange
            var mockContext = new Mock<FoodOrderingSystem.Models.IOnlineFoodDBEntities>();
            var data = new List<FoodOrderingSystem.Models.tblCart>().AsQueryable();
            var mockSet = GetMockDbSet(data);
            mockContext.Setup(c => c.tblCarts).Returns(mockSet.Object);

            var controller = new ShoppingCartController(mockContext.Object);
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

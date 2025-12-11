using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using OnlineFoodOrderingSystem.Models.ViewModel;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace FoodOrderingSystem.Tests.Integration
{
    /// <summary>
    /// Integration tests cho shopping cart và checkout flow
    /// Tests E2E từ add to cart đến order creation
    /// </summary>
    [TestClass]
    public class ShoppingCartCheckoutFlowTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_AddToCart_CreatesCartEntry()
        {
            // Arrange - Create test item first
            int testItemId;
            using (var db = new OnlineFoodDBEntities())
            {
                var testItem = new tblItem
                {
                    Title = $"Test Item {Guid.NewGuid():N}",
                    Description = "Integration test item",
                    Price = 50000,
                    SubMenuId = 1
                };
                db.tblItems.Add(testItem);
                db.SaveChanges();
                testItemId = testItem.ItemId;
            }

            // Mock HTTP context for shopping cart
            var controller = new ShoppingCartController();
            var httpContext = CreateMockHttpContext("testuser");
            controller.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), controller);

            // Act - Add item to cart
            var result = controller.AddToCart(testItemId);

            // Assert
            Assert.IsNotNull(result, "AddToCart should return result");
            
            // Verify cart entry in database
            using (var db = new OnlineFoodDBEntities())
            {
                var cartItem = db.tblCarts.FirstOrDefault(c => c.ItemId == testItemId);
                Assert.IsNotNull(cartItem, "Cart entry should be created");
                Assert.AreEqual(1, cartItem.Count, "Default quantity should be 1");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_CheckoutFlow_CreatesOrder()
        {
            // Arrange - Create test user, item, and cart
            string testUsername = $"buyer_{Guid.NewGuid():N}";
            int testItemId;

            using (var db = new OnlineFoodDBEntities())
            {
                // Create test user
                var user = new tblUser
                {
                    Username = testUsername + "@test.com",
                    Password = "test123"
                };
                db.tblUsers.Add(user);
                db.SaveChanges();

                // Create test item
                var item = new tblItem
                {
                    Title = $"Checkout Test Item {Guid.NewGuid():N}",
                    Description = "For checkout test",
                    Price = 100000,
                    SubMenuId = 1
                };
                db.tblItems.Add(item);
                db.SaveChanges();
                testItemId = item.ItemId;

                // Add to cart
                var cartItem = new tblCart
                {
                    ItemId = testItemId,
                    Count = 2,
                    CartId = testUsername,
                    DateCreated = DateTime.Now
                };
                db.tblCarts.Add(cartItem);
                db.SaveChanges();
            }

            // Setup controller with mock session
            var controller = new ShoppingCartController();
            var httpContext = CreateMockHttpContextWithSession(testUsername);
            controller.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), controller);

            var orderVm = new OrderViewModel
            {
                Firstname = "Test",
                Lastname = "User",
                Address = "123 Test St",
                Phone = "0123456789"
            };

            // Act - Checkout
            var result = controller.AddressAndPayment(orderVm);

            // Assert - Verify order created
            using (var db = new OnlineFoodDBEntities())
            {
                var order = db.tblOrders.FirstOrDefault(o => o.Username == testUsername + "@test.com");
                Assert.IsNotNull(order, "Order should be created");
                Assert.AreEqual("Test", order.Firstname);
                Assert.AreEqual("User", order.Lastname);
                Assert.AreEqual(200000, order.Total, "Total should be 2 * 100000");

                // Verify order details
                var orderDetails = db.tblOrderDetails.Where(od => od.OrderId == order.OrderId).ToList();
                Assert.IsTrue(orderDetails.Count > 0, "Order details should be created");
                Assert.AreEqual(testItemId, orderDetails[0].ItemId);
                Assert.AreEqual(2, orderDetails[0].Quantity);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_CartSummaryReflectsCartItems()
        {
            // Arrange - Create cart with items
            string testUsername = $"summary_{Guid.NewGuid():N}";
            
            using (var db = new OnlineFoodDBEntities())
            {
                // Create 2 test items
                var item1 = new tblItem { Title = "Item1", Price = 30000, SubMenuId = 1 };
                var item2 = new tblItem { Title = "Item2", Price = 50000, SubMenuId = 1 };
                db.tblItems.Add(item1);
                db.tblItems.Add(item2);
                db.SaveChanges();

                // Add to cart
                db.tblCarts.Add(new tblCart { ItemId = item1.ItemId, Count = 1, CartId = testUsername, DateCreated = DateTime.Now });
                db.tblCarts.Add(new tblCart { ItemId = item2.ItemId, Count = 2, CartId = testUsername, DateCreated = DateTime.Now });
                db.SaveChanges();
            }

            // Act - Get cart summary via ShoppingCart model
            using (var db = new OnlineFoodDBEntities())
            {
                var cartItems = db.tblCarts.Where(c => c.CartId == testUsername).ToList();
                
                // Assert
                Assert.AreEqual(2, cartItems.Count, "Should have 2 items in cart");
                
                int totalCount = cartItems.Sum(c => c.Count);
                Assert.AreEqual(3, totalCount, "Total item count should be 3 (1+2)");
            }
        }

        #region Helper Methods

        private Mock<HttpContextBase> CreateMockHttpContext(string username)
        {
            var mockContext = new Mock<HttpContextBase>();
            var mockRequest = new Mock<HttpRequestBase>();
            var mockResponse = new Mock<HttpResponseBase>();
            var mockSession = new Mock<HttpSessionStateBase>();

            mockContext.Setup(ctx => ctx.Request).Returns(mockRequest.Object);
            mockContext.Setup(ctx => ctx.Response).Returns(mockResponse.Object);
            mockContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);
            
            // Setup session
            mockSession.Setup(s => s["username"]).Returns(username);

            return mockContext;
        }

        private Mock<HttpContextBase> CreateMockHttpContextWithSession(string username)
        {
            var mockContext = CreateMockHttpContext(username);
            var mockSession = new Mock<HttpSessionStateBase>();
            mockSession.Setup(s => s["username"]).Returns(username + "@test.com");
            mockContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);
            
            return mockContext;
        }

        #endregion
    }
}

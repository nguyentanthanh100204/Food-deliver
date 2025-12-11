using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Models;
using System;
using System.Linq;

namespace FoodOrderingSystem.Tests.Integration
{
    /// <summary>
    /// Integration tests cho shopping cart data operations
    /// Simplified to focus on database integrity without controller mocking
    /// </summary>
    [TestClass]
    public class ShoppingCartDataTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_CartEntry_CanBeCreatedAndRetrieved()
        {
            // Arrange - Create test item first
            int testItemId;
            string testCartId = $"cart_{Guid.NewGuid():N}";
            
            using (var db = new OnlineFoodDBEntities())
            {
                var testItem = new tblItem
                {
                    Title = $"Cart Test Item {Guid.NewGuid():N}",
                    Description = "Integration test item for cart",
                    Price = 50000,
                    SubMenuId = 1
                };
                db.tblItems.Add(testItem);
                db.SaveChanges();
                testItemId = testItem.ItemId;

                // Act - Add to cart (direct database operation)
                var cartItem = new tblCart
                {
                    ItemId = testItemId,
                    Count = 1,
                    CartId = testCartId,
                    DateCreated = DateTime.Now
                };
                db.tblCarts.Add(cartItem);
                db.SaveChanges();
            }

            // Assert - Verify cart entry exists
            using (var db = new OnlineFoodDBEntities())
            {
                var cartEntry = db.tblCarts.FirstOrDefault(c => c.ItemId == testItemId && c.CartId == testCartId);
                Assert.IsNotNull(cartEntry, "Cart entry should be created");
                Assert.AreEqual(1, cartEntry.Count, "Default quantity should be 1");
                Assert.AreEqual(testItemId, cartEntry.ItemId);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_OrderWithDetails_CanBeCreatedCompletely()
        {
            // Arrange - Create user, item
            string testUsername = $"buyer_{Guid.NewGuid():N}@test.com";
            int testItemId;
            int testOrderId;

            using (var db = new OnlineFoodDBEntities())
            {
                // Create test user
                var user = new tblUser
                {
                    Username = testUsername,
                    Password = "test123"
                };
                db.tblUsers.Add(user);
                db.SaveChanges();

                // Create test item
                var item = new tblItem
                {
                    Title = $"Order Test Item {Guid.NewGuid():N}",
                    Description = "For order creation test",
                    Price = 100000,
                    SubMenuId = 1
                };
                db.tblItems.Add(item);
                db.SaveChanges();
                testItemId = item.ItemId;

                // Act - Create order (simulating checkout)
                var order = new tblOrder
                {
                    Username = testUsername,
                    Firstname = "Test",
                    Lastname = "User",
                    Address = "123 Test St",
                    Phone = "0123456789",
                    Total = 200000, // 2 * 100000
                    OrderDate = DateTime.Today,
                    DeliveredStatus = "Pending"
                };
                db.tblOrders.Add(order);
                db.SaveChanges();
                testOrderId = order.OrderId;

                // Add order details
                var orderDetail = new tblOrderDetail
                {
                    OrderId = testOrderId,
                    ItemId = testItemId,
                    Quantity = 2,
                    UnitPrice = 100000
                };
                db.tblOrderDetails.Add(orderDetail);
                db.SaveChanges();
            }

            // Assert - Verify order and details created
            using (var db = new OnlineFoodDBEntities())
            {
                var order = db.tblOrders.Find(testOrderId);
                Assert.IsNotNull(order, "Order should be created");
                Assert.AreEqual("Test", order.Firstname);
                Assert.AreEqual("User", order.Lastname);
                Assert.AreEqual(200000, order.Total);

                // Verify order details
                var orderDetails = db.tblOrderDetails.Where(od => od.OrderId == testOrderId).ToList();
                Assert.AreEqual(1, orderDetails.Count, "Should have 1 order detail");
                Assert.AreEqual(testItemId, orderDetails[0].ItemId);
                Assert.AreEqual(2, orderDetails[0].Quantity);
                Assert.AreEqual(100000, orderDetails[0].UnitPrice);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_MultipleCartItems_CanBeQueriedAndSummed()
        {
            // Arrange - Create cart with multiple items
            string testCartId = $"summary_{Guid.NewGuid():N}";
            
            using (var db = new OnlineFoodDBEntities())
            {
                // Create 2 test items
                var item1 = new tblItem 
                { 
                    Title = $"Item1_{Guid.NewGuid():N}", 
                    Price = 30000, 
                    SubMenuId = 1 
                };
                var item2 = new tblItem 
                { 
                    Title = $"Item2_{Guid.NewGuid():N}", 
                    Price = 50000, 
                    SubMenuId = 1 
                };
                db.tblItems.Add(item1);
                db.tblItems.Add(item2);
                db.SaveChanges();

                // Add to cart with different quantities
                db.tblCarts.Add(new tblCart 
                { 
                    ItemId = item1.ItemId, 
                    Count = 1, 
                    CartId = testCartId, 
                    DateCreated = DateTime.Now 
                });
                db.tblCarts.Add(new tblCart 
                { 
                    ItemId = item2.ItemId, 
                    Count = 2, 
                    CartId = testCartId, 
                    DateCreated = DateTime.Now 
                });
                db.SaveChanges();
            }

            // Act & Assert - Query cart and verify totals
            using (var db = new OnlineFoodDBEntities())
            {
                var cartItems = db.tblCarts.Where(c => c.CartId == testCartId).ToList();
                
                Assert.AreEqual(2, cartItems.Count, "Should have 2 items in cart");
                
                int totalCount = cartItems.Sum(c => c.Count);
                Assert.AreEqual(3, totalCount, "Total item count should be 3 (1+2)");
            }
        }
    }
}

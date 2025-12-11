using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Models;

namespace FoodOrderingSystem.Tests.Integration
{
    /// <summary>
    /// Base class for integration tests using manual cleanup instead of transaction rollback
    /// Each test uses unique GUID-based identifiers for easy cleanup
    /// </summary>
    public abstract class ManualCleanupTestBase
    {
        protected string TestRunId;
        protected List<int> CreatedItemIds = new List<int>();
        protected List<int> CreatedUserIds = new List<int>();
        protected List<int> CreatedOrderIds = new List<int>();

        [TestInitialize]
        public void Setup()
        {
            // Generate unique test run ID
            TestRunId = $"TEST_{Guid.NewGuid():N}".Substring(0, 20);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                using (var db = new OnlineFoodDBEntities())
                {
                    // Clean up in reverse order of dependencies
                    
                    // 1. Order details (depends on orders)
                    if (CreatedOrderIds.Count > 0)
                    {
                        var orderDetails = db.tblOrderDetails
                            .Where(od => CreatedOrderIds.Contains(od.OrderId))
                            .ToList();
                        db.tblOrderDetails.RemoveRange(orderDetails);
                    }

                    // 2. Orders
                    if (CreatedOrderIds.Count > 0)
                    {
                        var orders = db.tblOrders
                            .Where(o => CreatedOrderIds.Contains(o.OrderId))
                            .ToList();
                        db.tblOrders.RemoveRange(orders);
                    }

                    // 3. Cart items (no dependencies)
                    var carts = db.tblCarts
                        .Where(c => c.CartId.StartsWith("TEST_"))
                        .ToList();
                    if (carts.Count > 0)
                        db.tblCarts.RemoveRange(carts);

                    // 4. User roles (depends on users)
                    if (CreatedUserIds.Count > 0)
                    {
                        var userRoles = db.UserRoles
                            .Where(ur => CreatedUserIds.Contains(ur.UserId))
                            .ToList();
                        db.UserRoles.RemoveRange(userRoles);
                    }

                    // 5. Users
                    if (CreatedUserIds.Count > 0)
                    {
                        var users = db.tblUsers
                            .Where(u => CreatedUserIds.Contains(u.UserId))
                            .ToList();
                        db.tblUsers.RemoveRange(users);
                    }

                    // 6. Items (cleanup last, most independent)
                    if (CreatedItemIds.Count > 0)
                    {
                        var items = db.tblItems
                            .Where(i => CreatedItemIds.Contains(i.ItemId))
                            .ToList();
                        db.tblItems.RemoveRange(items);
                    }

                    // Also cleanup any items with test prefix
                    var testItems = db.tblItems
                        .Where(i => i.Title.StartsWith(TestRunId))
                        .ToList();
                    if (testItems.Count > 0)
                        db.tblItems.RemoveRange(testItems);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log cleanup failure but don't fail test
                Console.WriteLine($"Cleanup warning: {ex.Message}");
            }
        }

        protected void TrackItem(int itemId)
        {
            CreatedItemIds.Add(itemId);
        }

        protected void TrackUser(int userId)
        {
            CreatedUserIds.Add(userId);
        }

        protected void TrackOrder(int orderId)
        {
            CreatedOrderIds.Add(orderId);
        }
    }
}

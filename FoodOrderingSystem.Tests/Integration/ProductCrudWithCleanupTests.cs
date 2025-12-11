using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Models;
using System;
using System.Linq;

namespace FoodOrderingSystem.Tests.Integration
{
    /// <summary>
    /// Integration test for product CRUD using manual cleanup
    /// Test 1 of new approach - verify manual cleanup works on CI
    /// </summary>
    [TestClass]
    public class ProductCrudWithCleanupTests : ManualCleanupTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_CreateProduct_ThenCleanup()
        {
            // Arrange
            var uniqueTitle = $"{TestRunId}_Product";
            int createdId = 0;

            // Act - Create product
            using (var db = new OnlineFoodDBEntities())
            {
                var product = new tblItem
                {
                    Title = uniqueTitle,
                    Description = "Test product with manual cleanup",
                    Price = 150000,
                    SubMenuId = 1
                };
                db.tblItems.Add(product);
                db.SaveChanges();
                
                createdId = product.ItemId;
                TrackItem(createdId); // Track for cleanup
            }

            // Assert - Verify created
            using (var db = new OnlineFoodDBEntities())
            {
                var created = db.tblItems.Find(createdId);
                Assert.IsNotNull(created, "Product should be created");
                Assert.AreEqual(uniqueTitle, created.Title);
                Assert.AreEqual(150000, created.Price);
            }

            // Cleanup happens automatically in [TestCleanup]
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_UpdateProduct_ThenCleanup()
        {
            // Arrange - Create product
            var uniqueTitle = $"{TestRunId}_UpdateTest";
            int productId = 0;

            using (var db = new OnlineFoodDBEntities())
            {
                var product = new tblItem
                {
                    Title = uniqueTitle,
                    Description = "Original",
                    Price = 100000,
                    SubMenuId = 1
                };
                db.tblItems.Add(product);
                db.SaveChanges();
                productId = product.ItemId;
                TrackItem(productId);
            }

            // Act - Update product
            using (var db = new OnlineFoodDBEntities())
            {
                var product = db.tblItems.Find(productId);
                product.Description = "Updated";
                product.Price = 120000;
                db.SaveChanges();
            }

            // Assert - Verify updated
            using (var db = new OnlineFoodDBEntities())
            {
                var updated = db.tblItems.Find(productId);
                Assert.AreEqual("Updated", updated.Description);
                Assert.AreEqual(120000, updated.Price);
            }

            // Cleanup happens automatically
        }
    }
}

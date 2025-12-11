using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Models;
using System;
using System.Linq;

namespace FoodOrderingSystem.Tests.Integration
{
    /// <summary>
    /// Integration tests cho product CRUD operations
    /// Pure database operations - no controller dependencies
    /// </summary>
    [TestClass]
    public class AdminProductCrudTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_CreateProduct_PersistsInDatabase()
        {
            // Arrange
            var uniqueTitle = $"Product_{Guid.NewGuid():N}";
            var testProduct = new tblItem
            {
                Title = uniqueTitle,
                Description = "Test product",
                Price = 150000,
                SubMenuId = 1
            };

            // Act
            using (var db = new OnlineFoodDBEntities())
            {
                db.tblItems.Add(testProduct);
                db.SaveChanges();
            }

            // Assert
            using (var db = new OnlineFoodDBEntities())
            {
                var created = db.tblItems.FirstOrDefault(p => p.Title == uniqueTitle);
                Assert.IsNotNull(created);
                Assert.AreEqual(150000, created.Price);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_UpdateProduct_ChangesArePersisted()
        {
            // Arrange & Act
            var uniqueTitle = $"Update_{Guid.NewGuid():N}";
            int productId;
            
            using (var db = new OnlineFoodDBEntities())
            {
                var product = new tblItem
                {
                    Title = uniqueTitle,
                    Price = 100000,
                    SubMenuId = 1
                };
                db.tblItems.Add(product);
                db.SaveChanges();
                productId = product.ItemId;

                product.Price = 120000;
                db.SaveChanges();
            }

            // Assert
            using (var db = new OnlineFoodDBEntities())
            {
                var updated = db.tblItems.Find(productId);
                Assert.AreEqual(120000, updated.Price);
            }
        }
    }
}

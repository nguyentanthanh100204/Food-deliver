using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FoodOrderingSystem.Tests.Integration
{
    /// <summary>
    /// Integration tests cho product search và browse functionality
    /// Tests E2E search với database queries
    /// </summary>
    [TestClass]
    public class ProductSearchFlowTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_SearchProductReturnsMatchingItems()
        {
            // Arrange - Create test items with known titles
            var uniqueKeyword = $"TESTSEARCH{Guid.NewGuid():N}".Substring(0, 15);
            
            using (var db = new OnlineFoodDBEntities())
            {
                var item1 = new tblItem
                {
                    Title = $"{uniqueKeyword} Pizza",
                    Description = "Test pizza",
                    Price = 120000,
                    SubMenuId = 1
                };
                var item2 = new tblItem
                {
                    Title = $"{uniqueKeyword} Burger",
                    Description = "Test burger",
                    Price = 80000,
                    SubMenuId = 1
                };
                var item3 = new tblItem
                {
                    Title = "Other Food",
                    Description = $"Contains {uniqueKeyword} in description",
                    Price = 60000,
                    SubMenuId = 1
                };
                
                db.tblItems.Add(item1);
                db.tblItems.Add(item2);
                db.tblItems.Add(item3);
                db.SaveChanges();
            }

            // Act - Search for unique keyword
            var controller = new HomeController();
            var result = controller.ProductList(uniqueKeyword, 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result, "Search should return ViewResult");
            Assert.IsNotNull(model, "Model should not be null");
            Assert.IsTrue(model.Count >= 3, $"Should find at least 3 items with '{uniqueKeyword}'");

            // Verify all results contain the search keyword
            foreach (var item in model)
            {
                bool hasKeywordInTitle = item.Title.Contains(uniqueKeyword);
                bool hasKeywordInDescription = item.Description.Contains(uniqueKeyword);
                Assert.IsTrue(hasKeywordInTitle || hasKeywordInDescription,
                    $"Item '{item.Title}' should contain search keyword in title or description");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_EmptySearchReturnsAllItems()
        {
            // Arrange
            var controller = new HomeController();
            
            // Get count of all items in database
            int totalItemCount;
            using (var db = new OnlineFoodDBEntities())
            {
                totalItemCount = db.tblItems.Count();
            }

            // Act - Search with empty string
            var result = controller.ProductList("", 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.IsTrue(model.TotalItemCount >= totalItemCount,
                "Empty search should return all items");
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_SearchNonExistentProductReturnsEmpty()
        {
            // Arrange
            var controller = new HomeController();
            var impossibleKeyword = $"NOTEXIST{Guid.NewGuid():N}";

            // Act
            var result = controller.ProductList(impossibleKeyword, 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result, "Should return result even if no matches");
            Assert.IsNotNull(model, "Model should not be null");
            Assert.AreEqual(0, model.Count, "Should return empty list for non-existent product");
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_SearchBySubMenuIdFiltersCorrectly()
        {
            // Arrange - Create items in specific submenu
            int testSubMenuId = 5; // Use a specific submenu ID
            var uniqueTitle = $"SubMenu{Guid.NewGuid():N}".Substring(0, 15);
            
            using (var db = new OnlineFoodDBEntities())
            {
                // Create item in target submenu
                var targetItem = new tblItem
                {
                    Title = $"{uniqueTitle} Target",
                    Price = 70000,
                    SubMenuId = testSubMenuId
                };
                
                // Create item in different submenu
                var otherItem = new tblItem
                {
                    Title = $"{uniqueTitle} Other",
                    Price = 70000,
                    SubMenuId = testSubMenuId + 1
                };
                
                db.tblItems.Add(targetItem);
                db.tblItems.Add(otherItem);
                db.SaveChanges();
            }

            // Act - Search by submenu ID
            var controller = new HomeController();
            var result = controller.ProductList(null, 1, testSubMenuId) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            
            // Verify all returned items are from correct submenu
            foreach (var item in model)
            {
                Assert.AreEqual(testSubMenuId, item.SubMenuId,
                    "All items should be from the specified submenu");
            }
        }
    }
}

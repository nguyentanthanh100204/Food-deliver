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
    /// Integration tests cho product search functionality
    /// Tests search via HomeController with real database
    /// </summary>
    [TestClass]
    public class ProductSearchFlowTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_SearchProductReturnsMatchingItems()
        {
            // Arrange - Create test items
            var keyword = $"TEST{Guid.NewGuid():N}".Substring(0, 10);
            
            using (var db = new OnlineFoodDBEntities())
            {
                db.tblItems.Add(new tblItem
                {
                    Title = $"{keyword} Pizza",
                    Description = "Test",
                    Price = 100000,
                    SubMenuId = 1
                });
                db.SaveChanges();
            }

            // Act - Search via controller
            var controller = new HomeController();
            var result = controller.ProductList(keyword, 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Count >= 1, $"Should find items with '{keyword}'");
        }

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Integration_EmptySearchReturnsAllItems()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.ProductList("", 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
        }
    }
}

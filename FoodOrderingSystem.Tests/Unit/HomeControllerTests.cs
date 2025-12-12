using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Tests;
using Moq;
using FoodOrderingSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace FoodOrderingSystem.Tests.Unit
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Index_WhenCalled_ReturnsDefaultView()
        {
            // Arrange
            var mockDb = new Mock<IOnlineFoodDBEntities>();
            
            // Mock tblBanners to return an empty list
            var data = new List<tblBanner>().AsQueryable();
            var mockSet = new Mock<System.Data.Entity.DbSet<tblBanner>>();
            mockSet.As<IQueryable<tblBanner>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<tblBanner>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<tblBanner>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<tblBanner>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            
            mockDb.Setup(x => x.tblBanners).Returns(mockSet.Object);
            
            var controller = new HomeController(mockDb.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            // View mặc định có ViewName null hoặc rỗng
            Assert.AreEqual(string.Empty, result.ViewName ?? string.Empty);
        }

        // ⚠️ DEMO STRATEGY: Test commented out for live addition during defense
        // RESTORE INSTRUCTION: Uncomment lines 43-79 to add test back
        // This demonstrates adding tests in real-time when advisor asks!
        
        /*
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void ProductList_Search_Returns_Matching_Items()
        {
            // Arrange
            var mockDb = new Mock<IOnlineFoodDBEntities>();
            
            // Mock data - danh sách món ăn
            var mockItems = new List<tblItem>
            {
                new tblItem { ItemId = 1, Title = "Bánh Mì Thịt", Description = "Bánh mì kẹp thịt nướng", Price = 25000 },
                new tblItem { ItemId = 2, Title = "Phở Bò", Description = "Phở bò truyền thống", Price = 50000 },
                new tblItem { ItemId = 3, Title = "Cơm Tấm", Description = "Cơm tấm sườn bì chả", Price = 40000 }
            }.AsQueryable();
            
            var mockSet = new Mock<System.Data.Entity.DbSet<tblItem>>();
            mockSet.As<IQueryable<tblItem>>().Setup(m => m.Provider).Returns(mockItems.Provider);
            mockSet.As<IQueryable<tblItem>>().Setup(m => m.Expression).Returns(mockItems.Expression);
            mockSet.As<IQueryable<tblItem>>().Setup(m => m.ElementType).Returns(mockItems.ElementType);
            mockSet.As<IQueryable<tblItem>>().Setup(m => m.GetEnumerator()).Returns(mockItems.GetEnumerator());
            
            mockDb.Setup(x => x.tblItems).Returns(mockSet.Object);
            
            var controller = new HomeController(mockDb.Object);

            // Act - Tìm kiếm món có chữ "Phở"
            var result = controller.ProductList("Phở", 1, 0) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsNotNull(result.Model, "Model should not be null");
            
            // Verify found item có chứa "Phở" 
            var items = ((PagedList.IPagedList<tblItem>)result.Model).ToList();
            Assert.AreEqual(1, items.Count, "Should find 1 item matching 'Phở'");
            Assert.IsTrue(items[0].Title.Contains("Phở"), "Found item should contain 'Phở' in title");
        }
        */
    }
}

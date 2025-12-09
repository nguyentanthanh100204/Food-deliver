using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace FoodOrderingSystem.Tests.Unit
{
    /// <summary>
    /// Tests cho edge cases của tính năng tìm kiếm món ăn
    /// </summary>
    [TestClass]
    public class ProductSearchEdgeCaseTests
    {
        private Mock<IOnlineFoodDBEntities> _mockDb;
        private HomeController _controller;
        private List<tblItem> _mockItems;

        [TestInitialize]
        public void Setup()
        {
            // Arrange - Setup mock data
            _mockItems = new List<tblItem>
            {
                new tblItem 
                { 
                    ItemId = 1, 
                    Title = "Bánh Mì Thịt", 
                    Description = "Bánh mì kẹp thịt nướng",
                    Price = 25000,
                    SubMenuId = 1
                },
                new tblItem 
                { 
                    ItemId = 2, 
                    Title = "Phở Bò", 
                    Description = "Phở bò Hà Nội truyền thống",
                    Price = 50000,
                    SubMenuId = 1
                },
                new tblItem 
                { 
                    ItemId = 3, 
                    Title = "Cơm Tấm", 
                    Description = "Cơm tấm sườn bì chả",
                    Price = 40000,
                    SubMenuId = 1
                },
                new tblItem 
                { 
                    ItemId = 4, 
                    Title = "Pizza Hải Sản", 
                    Description = "Pizza với tôm, mực, nghêu",
                    Price = 120000,
                    SubMenuId = 2
                }
            };

            // Setup mock database
            _mockDb = new Mock<IOnlineFoodDBEntities>();
            var mockDbSet = CreateMockDbSet(_mockItems.AsQueryable());
            _mockDb.Setup(x => x.tblItems).Returns(mockDbSet.Object);

            // Create controller with mock
            _controller = new HomeController(_mockDb.Object);
        }

        #region Edge Case Tests

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void ProductList_Search_EmptyString_Returns_AllItems()
        {
            // Arrange
            string searchTerm = "";

            // Act
            var result = _controller.ProductList(searchTerm, 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result, "Result không được null");
            Assert.IsNotNull(model, "Model không được null");
            Assert.AreEqual(4, model.Count, "Search rỗng nên trả về tất cả 4 món");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void ProductList_Search_Null_Returns_AllItems()
        {
            // Arrange
            string searchTerm = null;

            // Act
            var result = _controller.ProductList(searchTerm, 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result, "Result không được null");
            Assert.IsNotNull(model, "Model không được null");
            Assert.AreEqual(4, model.Count, "Search null nên trả về tất cả 4 món");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void ProductList_Search_NotFound_Returns_EmptyList()
        {
            // Arrange
            string searchTerm = "Sushi"; // Món không có trong database

            // Act
            var result = _controller.ProductList(searchTerm, 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result, "Result không được null");
            Assert.IsNotNull(model, "Model không được null");
            Assert.AreEqual(0, model.Count, "Không tìm thấy 'Sushi' nên kết quả phải rỗng");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void ProductList_Search_SpecialCharacters_NoErrors()
        {
            // Arrange
            string searchTerm = "@#$%^&*()"; // Ký tự đặc biệt

            // Act
            var result = _controller.ProductList(searchTerm, 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result, "Result không được null - không crash với ký tự đặc biệt");
            Assert.IsNotNull(model, "Model không được null");
            Assert.AreEqual(0, model.Count, "Ký tự đặc biệt không match món nào");
            // Test PASS = Không crash, xử lý an toàn (SQL injection prevention)
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void ProductList_Search_CaseSensitive_Behavior()
        {
            // Arrange
            string searchTerm = "PHỞ"; // Viết hoa

            // Act
            var result = _controller.ProductList(searchTerm, 1, 0) as ViewResult;
            var model = result.Model as IPagedList<tblItem>;

            // Assert
            Assert.IsNotNull(result, "Result không được null");
            Assert.IsNotNull(model, "Model không được null");
            
            // Note: Contains() trong C# là case-sensitive by default
            // Test này document hành vi thực tế của system
            // Nếu muốn case-insensitive, cần modify controller code
            Assert.AreEqual(0, model.Count, "Search case-sensitive: 'PHỞ' không match 'Phở Bò'");
        }

        #endregion

        #region Helper Methods

        private Mock<IDbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<IDbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }

        #endregion
    }
}

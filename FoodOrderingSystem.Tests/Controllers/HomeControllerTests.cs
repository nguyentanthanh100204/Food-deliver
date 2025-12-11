using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using System.Collections.Generic;
using FoodOrderingSystem.Tests; // TestCategories
using Moq;
using System.Linq;
using System.Data.Entity;


namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
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

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Index_WhenCalled_ReturnsIndexView()
        {
            // Arrange
            var data = new List<tblBanner>().AsQueryable();
            var mockSet = GetMockDbSet(data);
            var mockContext = new Mock<IOnlineFoodDBEntities>();
            mockContext.Setup(c => c.tblBanners).Returns(mockSet.Object);

            var controller = new HomeController(mockContext.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            // ViewName rỗng nghĩa là trả về view mặc định (Index)
            Assert.AreEqual(string.Empty, result.ViewName ?? string.Empty);
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]  // 🔧 FIXED: Was missing, causing test to be skipped in CI!
        public void Index_WhenCalled_ModelIsListOfBanners()
        {
            // Arrange
            var data = new List<tblBanner>
            {
                new tblBanner(),
                new tblBanner()
            }.AsQueryable();
            var mockSet = GetMockDbSet(data);
            var mockContext = new Mock<IOnlineFoodDBEntities>();
            mockContext.Setup(c => c.tblBanners).Returns(mockSet.Object);

            var controller = new HomeController(mockContext.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<tblBanner>));
            Assert.AreEqual(2, ((IEnumerable<tblBanner>)result.Model).Count());
        }
    }
}

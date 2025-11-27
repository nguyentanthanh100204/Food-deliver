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
    }
}

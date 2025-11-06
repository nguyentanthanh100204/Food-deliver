using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using FoodOrderingSystem.Controllers;
using FoodOrderingSystem.Models;
using System.Collections.Generic;

namespace FoodOrderingSystem.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Index_WhenCalled_ReturnsIndexView()
        {
            var controller = new HomeController();

            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            // ViewName rỗng nghĩa là trả về view mặc định (Index)
            Assert.AreEqual(string.Empty, result.ViewName ?? string.Empty);
        }

        [TestMethod]
        public void Index_WhenCalled_ModelIsListOfBanners()
        {
            var controller = new HomeController();

            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<tblBanner>));
        }
    }
}

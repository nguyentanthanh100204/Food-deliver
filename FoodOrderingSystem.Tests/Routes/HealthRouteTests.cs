using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Mvc;      // (để dùng UrlParameter nếu cần)
using System.Web.Routing;
using Moq;
using FoodOrderingSystem;  // RouteConfig.RegisterRoutes
using FoodOrderingSystem.Tests; // TestCategories


namespace FoodOrderingSystem.Tests.Routes
{
    [TestClass]
    public class HealthRouteTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]

        public void HealthUrl_Maps_To_Health_Index()
        {
            // Arrange: đăng ký route giống lúc app khởi động
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            // Giả lập HttpContext cho URL "~/Health"
            var req = new Mock<HttpRequestBase>();
            req.SetupGet(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/Health");
            req.SetupGet(r => r.PathInfo).Returns(string.Empty);
            req.SetupGet(r => r.ApplicationPath).Returns("/");   // quan trọng!

            var ctx = new Mock<HttpContextBase>();
            ctx.SetupGet(c => c.Request).Returns(req.Object);

            // Act
            var data = routes.GetRouteData(ctx.Object);

            // Assert
            Assert.IsNotNull(data, "RouteData should not be null for /Health");
            Assert.AreEqual("Health", (string)data.Values["controller"]);
            Assert.AreEqual("Index",  (string)data.Values["action"]);
        }
    }
}

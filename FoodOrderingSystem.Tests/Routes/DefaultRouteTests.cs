using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Routing;
using Moq;
using FoodOrderingSystem; // RouteConfig.RegisterRoutes
using FoodOrderingSystem.Tests; // TestCategories


namespace FoodOrderingSystem.Tests.Routes
{
    [TestClass]
    public class DefaultRouteTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]

        public void RootUrl_Maps_To_Home_Index()
        {
            // Arrange
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            var httpCtx = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/");
            request.Setup(r => r.PathInfo).Returns(string.Empty);
            httpCtx.Setup(c => c.Request).Returns(request.Object);

            // Act
            var data = routes.GetRouteData(httpCtx.Object);

            // Assert
            Assert.IsNotNull(data);
            Assert.AreEqual("Home", data.Values["controller"]);
            Assert.AreEqual("Index", data.Values["action"]);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Routing;
using Moq;
using FoodOrderingSystem;

namespace FoodOrderingSystem.Tests.Routes
{
    [TestClass]
    public class HealthRouteTests
    {
        [TestMethod]
        public void HealthUrl_Maps_To_Health_Index()
        {
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            var ctx = new Mock<HttpContextBase>();
            var req = new Mock<HttpRequestBase>();
            req.Setup(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/Health");
            req.Setup(r => r.PathInfo).Returns(string.Empty);
            ctx.Setup(c => c.Request).Returns(req.Object);

            var data = routes.GetRouteData(ctx.Object);

            Assert.IsNotNull(data);
            Assert.AreEqual("Health", data.Values["controller"]);
            Assert.AreEqual("Index", data.Values["action"]);
        }
    }
}

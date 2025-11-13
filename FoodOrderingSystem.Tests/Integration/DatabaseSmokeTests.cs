using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FoodOrderingSystem.Models;   // <-- đúng namespace
using FoodOrderingSystem.Tests;                 // để dùng TestCategories
using FoodOrderingSystem.Tests.Integration;     // để kế thừa IntegrationTestBase


namespace FoodOrderingSystem.Tests.Integration
{
    [TestClass]
    public class DatabaseSmokeTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        public void Can_Query_tblBanners_Count()
        {
            using (var db = new OnlineFoodDBEntities())
            {
                var count = db.tblBanners.Count();   // xác nhận kết nối + schema OK
                Assert.IsTrue(count >= 0);
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FoodOrderingSystem.Models;   // <-- đúng namespace

namespace FoodOrderingSystem.Tests.Integration
{
    [TestClass]
    public class DatabaseSmokeTests
    {
        [TestMethod]
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

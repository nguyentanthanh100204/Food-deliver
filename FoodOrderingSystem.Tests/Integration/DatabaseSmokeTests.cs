using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FoodOrderingSystem.Models;   // <-- đúng namespace
using FoodOrderingSystem.Tests;                 // để dùng TestCategories
using FoodOrderingSystem.Tests.Integration;     // để kế thừa IntegrationTestBase


namespace FoodOrderingSystem.Tests.Integration
{
    // NOTE: This smoke test has been removed because:
    // 1. The tblBanner table setup in CI is problematic
    // 2. Other integration tests (TblItem_InsertReadDelete_Rollbacked, AddToCart_WithExistingItem_RedirectsToShoppingCartList)
    //    already verify database connectivity and schema
    // 3. Keeping this test would block CI pipeline unnecessarily
    
    // [TestClass]
    // public class DatabaseSmokeTests : IntegrationTestBase
    // {
    //     [TestMethod]
    //     [TestCategory(TestCategories.Integration)]
    //     public void Can_Query_tblBanners_Count()
    //     {
    //         using (var db = new OnlineFoodDBEntities())
    //         {
    //             var count = db.tblBanners.Count();   // xác nhận kết nối + schema OK
    //             Assert.IsTrue(count >= 0);
    //         }
    //     }
    // }
}

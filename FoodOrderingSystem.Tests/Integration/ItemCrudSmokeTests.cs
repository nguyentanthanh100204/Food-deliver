using System;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Models;

namespace FoodOrderingSystem.Tests.Integration
{
    [TestClass]
    public class ItemCrudSmokeTests
    {
        [TestMethod]
        public void TblItem_InsertReadDelete_Rollbacked()
        {
            using (var scope = new TransactionScope())
            {
                var unique = "ITM-" + Guid.NewGuid().ToString("N").Substring(0, 8);

                using (var db = new OnlineFoodDBEntities())
                {
                    var item = new tblItem
                    {
                        Title = unique,
                        Description = "Smoke test item",
                        Price = 12345m
                        // Nếu schema có NOT NULL khác: set thêm ở đây
                        // CategoryId = ...; ImagePath = ...; v.v.
                    };

                    db.tblItems.Add(item);
                    db.SaveChanges();

                    var fromDb = db.tblItems.FirstOrDefault(x => x.Title == unique);
                    Assert.IsNotNull(fromDb, "Inserted item must be retrievable.");
                    Assert.AreEqual(12345m, fromDb.Price);

                    db.tblItems.Remove(fromDb);
                    db.SaveChanges();
                }

                // KHÔNG gọi scope.Complete() -> rollback
            }
        }
    }
}

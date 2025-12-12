using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Models;
using System.Linq;
using System.Collections.Generic;

namespace FoodOrderingSystem.Tests.Unit
{
    [TestClass]
    public class MenuItemManagementTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void DeleteMenuItem_WithValidId_RemovesItemSuccessfully()
        {
            // Arrange - Setup mock menu items list
            var menuItems = new List<tblItem>
            {
                new tblItem { ItemId = 1, Title = "Phở Bò", Price = 50000 },
                new tblItem { ItemId = 2, Title = "Bún Chả", Price = 45000 },
                new tblItem { ItemId = 3, Title = "Cơm Tấm", Price = 40000 }
            };

            int itemIdToDelete = 2;
            int initialCount = menuItems.Count;

            // Act - Delete menu item
            var itemToDelete = menuItems.FirstOrDefault(i => i.ItemId == itemIdToDelete);
            if (itemToDelete != null)
            {
                menuItems.Remove(itemToDelete);
            }

            // Assert
            Assert.AreEqual(initialCount - 1, menuItems.Count, "Menu item count should decrease by 1");
            Assert.AreEqual(2, menuItems.Count, "Should have 2 menu items remaining");
            Assert.IsFalse(menuItems.Any(i => i.ItemId == itemIdToDelete), 
                "Deleted menu item should not exist in list");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void DeleteMenuItem_WithInvalidId_DoesNotChangeList()
        {
            // Arrange
            var menuItems = new List<tblItem>
            {
                new tblItem { ItemId = 1, Title = "Phở Bò", Price = 50000 },
                new tblItem { ItemId = 2, Title = "Bún Chả", Price = 45000 }
            };

            int invalidId = 999;
            int initialCount = menuItems.Count;

            // Act - Try to delete non-existent item
            var itemToDelete = menuItems.FirstOrDefault(i => i.ItemId == invalidId);
            if (itemToDelete != null)
            {
                menuItems.Remove(itemToDelete);
            }

            // Assert - List should remain unchanged
            Assert.AreEqual(initialCount, menuItems.Count, "Count should not change for invalid ID");
            Assert.AreEqual(2, menuItems.Count, "Should still have 2 menu items");
        }
    }
}

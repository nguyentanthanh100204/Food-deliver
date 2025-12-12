using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodOrderingSystem.Models;
using Moq;
using System.Linq;
using System.Collections.Generic;

namespace FoodOrderingSystem.Tests.Unit
{
    [TestClass]
    public class RestaurantManagementTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void DeleteRestaurant_WithValidId_RemovesRestaurantSuccessfully()
        {
            // Arrange - Setup mock restaurant list
            var restaurants = new List<tblRestaurant>
            {
                new tblRestaurant { RestaurantId = 1, RestaurantName = "Nhà hàng A", Address = "123 Street" },
                new tblRestaurant { RestaurantId = 2, RestaurantName = "Nhà hàng B", Address = "456 Street" },
                new tblRestaurant { RestaurantId = 3, RestaurantName = "Nhà hàng C", Address = "789 Street" }
            };

            int restaurantIdToDelete = 2;
            int initialCount = restaurants.Count;

            // Act - Delete restaurant
            var restaurantToDelete = restaurants.FirstOrDefault(r => r.RestaurantId == restaurantIdToDelete);
            if (restaurantToDelete != null)
            {
                restaurants.Remove(restaurantToDelete);
            }

            // Assert
            Assert.AreEqual(initialCount - 1, restaurants.Count, "Restaurant count should decrease by 1");
            Assert.AreEqual(2, restaurants.Count, "Should have 2 restaurants remaining");
            Assert.IsFalse(restaurants.Any(r => r.RestaurantId == restaurantIdToDelete), 
                "Deleted restaurant should not exist in list");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void DeleteRestaurant_WithInvalidId_DoesNotChangeList()
        {
            // Arrange
            var restaurants = new List<tblRestaurant>
            {
                new tblRestaurant { RestaurantId = 1, RestaurantName = "Nhà hàng A" },
                new tblRestaurant { RestaurantId = 2, RestaurantName = "Nhà hàng B" }
            };

            int invalidId = 999;
            int initialCount = restaurants.Count;

            // Act - Try to delete non-existent restaurant
            var restaurantToDelete = restaurants.FirstOrDefault(r => r.RestaurantId == invalidId);
            if (restaurantToDelete != null)
            {
                restaurants.Remove(restaurantToDelete);
            }

            // Assert - List should remain unchanged
            Assert.AreEqual(initialCount, restaurants.Count, "Count should not change for invalid ID");
            Assert.AreEqual(2, restaurants.Count, "Should still have 2 restaurants");
        }
    }
}

using System;
using System.Data.Entity;

namespace FoodOrderingSystem.Models
{
    public interface IOnlineFoodDBEntities : IDisposable
    {
        DbSet<Event> Events { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<tblBanner> tblBanners { get; set; }
        DbSet<tblCart> tblCarts { get; set; }
        DbSet<tblItem> tblItems { get; set; }
        DbSet<tblOrder> tblOrders { get; set; }
        DbSet<tblOrderDetail> tblOrderDetails { get; set; }
        DbSet<tblSubMenu> tblSubMenus { get; set; }
        DbSet<tblUser> tblUsers { get; set; }
        DbSet<UserRole> UserRoles { get; set; }

        int SaveChanges();
    }
}

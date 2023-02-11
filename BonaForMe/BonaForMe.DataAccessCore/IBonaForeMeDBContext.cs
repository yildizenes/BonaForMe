using BonaForMe.DomainCore.DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BonaForMe.DataAccessCore
{
    public interface IBonaForMeDBContext
    {
        DbSet<Announcement> Announcements { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<ContactInformation> ContactInformations { get; set; }
        DbSet<CurrencyUnit> CurrencyUnits { get; set; }
        DbSet<LinkOrderProduct> LinkOrderProducts { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderStatus> OrderStatuses { get; set; }
        DbSet<PaymentInfo> PaymentInfos { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<ProductUnit> ProductUnits { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        DbSet<CampaignProduct> CampaignProducts { get; set; }
        DbSet<CourierCoordinate> CourierCoordinates { get; set; }
        DbSet<SpecialPrice> SpecialPrices { get; set; }
        DbSet<OrderLog> OrderLogs { get; set; }
        DbSet<OrderDetail> OrderDetails { get; set; }

        int SaveChanges();
        EntityEntry Entry(object entity);
    }
}

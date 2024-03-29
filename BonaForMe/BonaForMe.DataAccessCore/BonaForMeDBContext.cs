﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BonaForMe.DomainCommonCore.CustomClass;
using BonaForMe.DomainCore.DBModel;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace BonaForMe.DataAccessCore
{
    public partial class BonaForMeDBContext : DbContext
    {
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContactInformation> ContactInformations { get; set; }
        public DbSet<CurrencyUnit> CurrencyUnits { get; set; }
        public DbSet<LinkOrderProduct> LinkOrderProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<PaymentInfo> PaymentInfos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductUnit> ProductUnits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<CampaignProduct> CampaignProducts { get; set; }
        public DbSet<CourierCoordinate> CourierCoordinates { get; set; }
        public DbSet<SpecialPrice> SpecialPrices { get; set; }
        public DbSet<OrderLog> OrderLogs { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<WorkPartner> WorkPartners { get; set; }
        public DbSet<OrderHour> OrderHours { get; set; }


        IHttpContextAccessor _httpContextAccessor;
        public BonaForMeDBContext(DbContextOptions<BonaForMeDBContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            Database.EnsureCreated();

        }
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }
        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => (x.Entity is BaseEntity || x.Entity is BaseEntityInt) && (x.State == EntityState.Added || x.State == EntityState.Modified));
            UserClaimsInfo user = null;
            foreach (var entity in entities)
            {
                Guid userid = Guid.Empty;
                try
                {
                    user = CurrentUser.Get(_httpContextAccessor.HttpContext.User);
                    userid = user != null ? Guid.Parse(user.UserId) : Guid.Parse("1FCC262D-5EA7-4BA6-890B-681AC0886971");
                }
                catch (Exception)
                {
                }
                if (entity.Entity is BaseEntity)
                {
                    if (entity.State == EntityState.Added)
                    {
                        ((BaseEntity)entity.Entity).Id = Guid.NewGuid();
                        ((BaseEntity)entity.Entity).DateCreated = DateTime.UtcNow.AddHours(1);
                        ((BaseEntity)entity.Entity).UserCreated = userid;
                        ((BaseEntity)entity.Entity).IsActive = true;
                        ((BaseEntity)entity.Entity).IsDeleted = false;
                    }

                    ((BaseEntity)entity.Entity).DateModified = DateTime.UtcNow.AddHours(1);
                    ((BaseEntity)entity.Entity).UserModified = userid;
                }
                if (entity.Entity is BaseEntityInt)
                {
                    if (entity.State == EntityState.Added)
                    {
                        ((BaseEntityInt)entity.Entity).DateCreated = DateTime.UtcNow.AddHours(1);
                        ((BaseEntityInt)entity.Entity).UserCreated = userid;
                        ((BaseEntityInt)entity.Entity).IsActive = true;
                        ((BaseEntityInt)entity.Entity).IsDeleted = false;
                    }

                    ((BaseEntityInt)entity.Entity).DateModified = DateTime.UtcNow.AddHours(1);
                    ((BaseEntityInt)entity.Entity).UserModified = userid;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<CourierCoordinate>().Property(o => o.XCoordinate).HasColumnType("decimal(18, 10)");
            modelBuilder.Entity<CourierCoordinate>().Property(o => o.YCoordinate).HasColumnType("decimal(18, 10)");
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
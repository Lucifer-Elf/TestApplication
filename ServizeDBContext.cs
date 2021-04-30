using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Servize.Authentication;
using Servize.Domain.Model;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Model.VendorModel;
using System;
using System.Linq;

namespace Servize
{
    public class ServizeDBContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<BookingSetting> BookingSetting { get; set; }
        public virtual DbSet<VendorBankDetail> VendorBankDetail { get; set; }
              
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<OrderSummary> OrderSummary { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<CartItem> CartItem { get; set; }
             
        public virtual DbSet<Cart> Cart { get; set; }
         
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }

        public ServizeDBContext(DbContextOptions<ServizeDBContext> options) :
            base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RefreshToken>(entity =>
            {
                entity.Property(i => i.Token).HasMaxLength(2000);

            });
            base.OnModelCreating(builder);
        }

        private static void UpdateModifiedProperty(EntityEntry entry)
        {
            if (entry == null) return;

            BaseEntity baseEntity = (BaseEntity)entry.Entity;
            if (baseEntity == null) return;

            var modifiedField = entry.Property(nameof(baseEntity.Modified));
            if (modifiedField != null)
            {
                modifiedField.OriginalValue = modifiedField.CurrentValue;
                modifiedField.CurrentValue = DateTime.Now;
            }
        }

        public void UpdateModifiedField()
        {
            if (ChangeTracker == null || ChangeTracker.Entries() == null || !ChangeTracker.Entries().Any()) return;

            var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                UpdateModifiedProperty(entityEntry);
            }
        }
    }
}

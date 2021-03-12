using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Servize.Authentication;
using Servize.Domain.Model;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Model.Provider;
using System;
using System.Linq;

namespace Servize
{
    public class ServizeDBContext : IdentityDbContext<ApplicationUser>
    {
     
        public DbSet<Category> Category { get; set; }
        public DbSet<Provider> Provider { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<BookingSetting> BookingSetting { get; set; }
        public DbSet<ProviderBankDetail> ProviderBankDetail { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<OrderSummary> OrderSummary { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<CartItem> CartItem { get; set; }

        public DbSet<Cart> Cart { get; set; }

        public ServizeDBContext(DbContextOptions<ServizeDBContext> options):
            base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        private void updateModifiedProperty(EntityEntry entry)
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
                updateModifiedProperty(entityEntry);
            }
        
        }

    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Servize.Authentication;
using Servize.Domain.Model.Provider;
using Servize.Domain.Model.Account;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Model.Client;

namespace Servize
{
    public class ServizeDBContext : IdentityDbContext<ApplicationUser>
    {
     
        public DbSet<ServizeCategory>       ServizeCategory                 { get; set; }
        public DbSet<ServizeProvider>       ServizeProvider                 { get; set; }
        public DbSet<ServizeReview>         ServizeReview                   { get; set; }
        public DbSet<ServizeSubCategory>    ServizeSubCategory              { get; set; }
        public DbSet<ServizeBookingSetting> ServizeBookingSetting           { get; set; }
        public DbSet<ServizeProviderBankDetail> ServizeProviderBankDetail   { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<OrderSummary> OrderSummary { get; set; }
        public DbSet<UserClient> UserClient { get; set; }
        public DbSet<CartItem> CartItem { get; set; }




        public ServizeDBContext(DbContextOptions<ServizeDBContext> options):
            base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using Servize.Domain.Model;

namespace Servize
{
    public class ServizeDBContext : DbContext
    {
        public DbSet<ServizeAdmin>          ServizeAdmin                  { get; set; }
        public DbSet<ServizeCategory>       ServizeCategory               { get; set; }
        public DbSet<ServizeProvider>       ServizeProvider               { get; set; }
        public DbSet<ServizeReview>         ServizeReview                 { get; set; }
        public DbSet<ServizeSubCategory>    ServizeSubCategory            { get; set; }
        public DbSet<ServizeUser>           ServizeUser                   { get; set; }


        public ServizeDBContext(DbContextOptions<ServizeDBContext> options):
            base(options)
        {

        }
       
    }
}

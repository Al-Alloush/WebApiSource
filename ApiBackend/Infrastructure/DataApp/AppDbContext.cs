using Core.Entities.AppSettings;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Infrastructure.DataApp
{
    /// <summary>
    /// this DB for Identity and all App settings and default data
    /// </summary>
    /// 
    public class AppDbContext : IdentityDbContext<AppUser, AppIdentityRole, string>
    {
        // this Constructor is required
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<AppLanguage> AppLanguages { get; set; }
    }
}

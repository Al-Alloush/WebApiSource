using Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.DataApp.Configuration
{
    public class IdentityConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            // delete UserImages record in database after Remove it from ICollection<UserPhoto>
            builder.HasMany(p => p.UserImages).WithOne().OnDelete(DeleteBehavior.Cascade);

            // uniqu key UserName
            builder.HasAlternateKey(u => u.UserName);

            // uniqu Fields Email 
            builder.HasIndex(u => u.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

            // Prevent delete language as long as there are users using this language
            builder.HasOne(x => x.Language).WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}

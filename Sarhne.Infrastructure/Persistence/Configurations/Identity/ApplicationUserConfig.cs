using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sarhne.Domain.Entities;
using Sarhne.Domain.Entities.Identity;

namespace Sarhne.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.RowVersion)
               .IsRowVersion();

        builder.Property(x => x.FullName)
       .IsRequired()
       .HasMaxLength(100);

        builder.Property(x => x.UserName)
       .IsRequired()
       .HasMaxLength(100);

        builder.Property(x => x.AccountLink)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.AccountLink)
            .IsUnique();

        builder.HasIndex(x => x.UserName)
            .IsUnique();

        builder.Property(x => x.AboutMe)
            .HasMaxLength(500);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Gender)
            .HasConversion<int>();

    }
}

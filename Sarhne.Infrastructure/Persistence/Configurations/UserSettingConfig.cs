using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sarhne.Domain.Entities;
using Sarhne.Infrastructure.Persistence;

namespace Sarhne.Infrastructure.Persistence.Configurations;

public class UserSettingConfig : GlobalConfiguration<UserSetting>
{
    public override void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.UserId)
       .IsUnique();

        builder.Property(x => x.AllowAnonymousMessages)
            .HasDefaultValue(true);

        builder.Property(x => x.AllowMessages)
            .HasDefaultValue(true);

        builder.Property(x => x.AllowSendPhoto)
            .HasDefaultValue(true);

        builder.Property(x => x.AllowReceiveEmail)
            .HasDefaultValue(true);

        builder.Property(x => x.AllowReceiveNotifications)
            .HasDefaultValue(true);

        builder.Property(x => x.ShowLastSeen)
            .HasDefaultValue(true);

        builder.Property(x => x.ShowProfileViewsCount)
            .HasDefaultValue(true);

        builder.HasOne(x => x.User)
            .WithOne(x => x.UserSetting)
            .HasForeignKey<UserSetting>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

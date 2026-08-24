using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sarhne.Domain.Entities;
using Sarhne.Infrastructure.Persistence;

namespace Sarhne.Infrastructure.Persistence.Configurations;

public class NotificationConfig : GlobalConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title)
        .IsRequired()
        .HasMaxLength(200);

        builder.Property(x => x.Body)
            .HasMaxLength(1000);

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.IsRead)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.ReceiverId);

        builder.HasIndex(x =>
        new
        {
            x.ReceiverId,
            x.CreatedAt
        });

        builder.HasOne(x => x.Sender)
            .WithMany(x => x.SentNotifications)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Receiver)
            .WithMany(x => x.ReceivedNotifications)
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

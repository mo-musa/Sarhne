using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sarhne.Domain.Entities;
using Sarhne.Infrastructure.Persistence;

namespace Sarhne.Infrastructure.Persistence.Configurations;

public class MessageConfig : GlobalConfiguration<Message>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Content)
        .HasMaxLength(5000);

        builder.Property(x => x.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.LikesCount)
            .HasDefaultValue(0);

        builder.Property(x => x.IsRead)
            .HasDefaultValue(false);

        builder.Property(x => x.IsStarred)
            .HasDefaultValue(false);

        builder.Property(x => x.IsHidden)
            .HasDefaultValue(true);

        builder.Property(x => x.IsAnonymous)
            .HasDefaultValue(true);

        builder.HasIndex(x =>
        new
        {
            x.ReceiverId,
            x.CreatedAt
        });

        builder.HasIndex(x => x.SenderId);
        builder.HasIndex(x => x.ReceiverId);

        builder.HasOne(x => x.Sender)
            .WithMany(x => x.SentMessages)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Receiver)
            .WithMany(x => x.ReceivedMessages)
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

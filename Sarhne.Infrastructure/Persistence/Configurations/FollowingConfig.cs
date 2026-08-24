using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;
using Sarhne.Domain.Entities;
using Sarhne.Infrastructure.Persistence;

namespace Sarhne.Infrastructure.Persistence.Configurations;

public class FollowingConfig : IEntityTypeConfiguration<Following>
{
    public  void Configure(EntityTypeBuilder<Following> builder)
    {
        builder.HasIndex(x => new
        {
            x.FollowerId,
            x.CreatorId
        }).IsUnique();

        builder.ToTable(t =>
            t.HasCheckConstraint(
                "CK_Following_NoSelfFollow",
                "[FollowerId] <> [CreatorId]")
        );
        builder.HasOne(x => x.Follower)
            .WithMany(x => x.Following)
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Creator)
            .WithMany(x => x.Followers)
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

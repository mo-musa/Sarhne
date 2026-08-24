using Sarhne.Domain.Common;
using Sarhne.Domain.Entities.Identity;

namespace Sarhne.Domain.Entities;

public class Following : AuditableEntity
{
    public int FollowerId { get; set; }

    public ApplicationUser Follower { get; set; } = null!;

    public int CreatorId { get; set; }

    public ApplicationUser Creator { get; set; } = null!;
    public Following()
    {
        
    }
    public Following(int followerId,int creatorId)
    {
        if (followerId == creatorId)
            throw new InvalidOperationException();

        FollowerId = followerId;
        CreatorId = creatorId;
    }
}

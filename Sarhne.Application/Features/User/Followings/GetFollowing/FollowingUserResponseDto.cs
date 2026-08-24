using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.GetFollowing;

public sealed class FollowingUserResponseDto
{
    public required string UserName { get; set; }

    public required string FullName { get; set; }

    public string? ImageUrl { get; set; }
}
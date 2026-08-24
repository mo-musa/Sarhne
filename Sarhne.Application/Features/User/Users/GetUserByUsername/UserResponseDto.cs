using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.GetUserByUsername;

public sealed class UserResponseDto
{
    public required string FullName { get; set; }
    public string? AboutMe { get; set; }
    public string? ImageUrl { get; set; }
    public Gender Gender { get; set; }
    public int FollowingCount { get; set; }
    public int FollowersCount { get; set; }
    public int? VisitorsCount { get; set; }
    public DateTime? LastSeen { get; set; }
    public bool IsFollowing { get; set; }
}
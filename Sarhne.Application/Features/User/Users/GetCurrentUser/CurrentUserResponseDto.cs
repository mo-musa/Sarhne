using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.GetCurrentUser;

public sealed class CurrentUserResponseDto
{
    public required string FullName { get; set; }
    public string? AboutMe { get; set; }
    public string? ImageUrl { get; set; }
    public Gender Gender { get; set; }
    public int FollowingCount { get; set; }
    public int FollowersCount { get; set; }
    public int? VisitorsCount { get; set; }
    public DateTime? LastSeen { get; set; }
}
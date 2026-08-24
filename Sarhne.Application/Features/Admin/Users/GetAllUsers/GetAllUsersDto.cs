using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Users.GetAllUsers;

public sealed class GetAllUsersDto
{
    public int Id { get; init; }

    public string UserName { get; init; } = null!;

    public string? FullName { get; init; }

    public Gender Gender { get; init; }

    public string? ImageUrl { get; init; }
    public List<string> Roles { get; set; } = new();
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.SuperAdmin.Role.GetAdmins;

public sealed class AdminDto
{
    public int Id { get; init; }

    public string UserName { get; init; } = null!;

    public string? Email { get; init; }

    public string? FullName { get; init; }

    public string? ImageUrl { get; init; }

    public DateTime CreatedAt { get; init; }
}
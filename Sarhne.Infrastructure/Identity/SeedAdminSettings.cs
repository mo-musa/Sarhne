using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Infrastructure.Identity;

public sealed class SeedAdminSettings
{
    public const string SectionName = "SeedAdmin";

    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;

    public string FullName { get; init; } = default!;

    public string UserName { get; init; } = default!;

    public Gender Gender { get; init; }
}

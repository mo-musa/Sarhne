using Microsoft.AspNetCore.Http;

namespace Sarhne.Infrastructure.Settings;

public sealed class CookieSettings
{
    public const string SectionName = "CookieSettings";

    public bool HttpOnly { get; init; }

    public bool Secure { get; init; }

    public SameSiteMode SameSite { get; init; }

    public string Path { get; init; } = "/";

    public string? Domain { get; init; }
}

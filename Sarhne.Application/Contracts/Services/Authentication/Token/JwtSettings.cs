using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Authentication.Token;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required]
    [MinLength(32)]
    public string SecretKey { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenExpirationInMinutes { get; init; }

    [Range(1, 365)]
    public int RefreshTokenExpirationInDays { get; init; }
}

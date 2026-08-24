using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sarhne.Infrastructure.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "Email";

    [Required]
    public string Host { get; init; } = default!;

    [Range(1, 65535)]
    public int Port { get; init; }

    public bool EnableSsl { get; init; }

    [Required]
    public string SenderName { get; init; } = default!;

    [Required]
    [EmailAddress]
    public string SenderEmail { get; init; } = default!;

    [Required]
    public string Password { get; init; } = default!;
}
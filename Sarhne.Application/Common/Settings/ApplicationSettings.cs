using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sarhne.Application.Common.Settings;

public sealed class ApplicationSettings
{
    public const string SectionName = "Application";

    [Required]
    public string FrontendUrl { get; init; } = default!;
}
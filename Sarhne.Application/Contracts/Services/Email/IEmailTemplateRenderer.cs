using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Email;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync(
        string templateName,
        IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken = default);
}
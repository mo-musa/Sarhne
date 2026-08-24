using Sarhne.Application.Contracts.Services.Email;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sarhne.Infrastructure.Services.Email;

public sealed class EmailTemplateRenderer : IEmailTemplateRenderer
{
    private static readonly Assembly Assembly =
        typeof(EmailTemplateRenderer).Assembly;

    public async Task<string> RenderAsync(
        string templateName,
        IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken = default)
    {
        var resourceName =
            $"Sarhne.Infrastructure.Services.Email.Templates.{templateName}.html";

        await using var stream =
            Assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            throw new FileNotFoundException(
                $"Email template '{templateName}' was not found.",
                resourceName);
        }

        using var reader = new StreamReader(stream);

        var html = await reader.ReadToEndAsync(cancellationToken);

        foreach (var placeholder in placeholders)
        {
            html = html.Replace(
                $"{{{{{placeholder.Key}}}}}",
                placeholder.Value);
        }

        return html;
    }
}
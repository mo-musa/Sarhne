using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Sarhne.Application.Contracts.Services.Email;
using Sarhne.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Infrastructure.Services.Email;

public sealed class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(
        EmailRequest message,
        CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));

        email.To.Add(
            MailboxAddress.Parse(message.To));

        email.Subject = message.Subject;

        email.Body = new BodyBuilder
        {
            HtmlBody = message.IsHtml
                ? message.Body
                : null,

            TextBody = message.IsHtml
                ? null
                : message.Body
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            _settings.EnableSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None,
            cancellationToken);

        await smtp.AuthenticateAsync(
            _settings.SenderEmail,
            _settings.Password,
            cancellationToken);

        await smtp.SendAsync(
            email,
            cancellationToken);

        await smtp.DisconnectAsync(
            true,
            cancellationToken);
    }
}
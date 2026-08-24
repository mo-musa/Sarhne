using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Sarhne.Application.Common.Constants;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Common.Settings;
using Sarhne.Application.Contracts.Services.Email;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.ForgotPassword;

public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly ApplicationSettings _applicationSettings;
    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IEmailTemplateRenderer templateRenderer,
        IOptions<ApplicationSettings> applicationSettings)
    {
        _userManager = userManager;
        _emailService = emailService;
        _templateRenderer = templateRenderer;
        _applicationSettings = applicationSettings.Value;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        // Don't reveal whether the email exists.
        if (user is null)
            return Result.Success();

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Result.Success();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

        var resetLink =
            $"{_applicationSettings.FrontendUrl}" +
            $"/reset-password" +
            $"?email={Uri.EscapeDataString(user.Email!)}" +
            $"&token={Uri.EscapeDataString(encodedToken)}";

        var body = await _templateRenderer.RenderAsync(
            EmailTemplates.ResetPassword,
            new Dictionary<string, string>
            {
                ["FullName"] = user.FullName ?? user.UserName!,
                ["ResetLink"] = resetLink,
                //____ devpro _____
                ["Email"] = user.Email!,
                ["ResetToken"] = encodedToken,
                //_________
            },
            cancellationToken);

        await _emailService.SendAsync(
            new EmailRequest(
                user.Email!,
                "Reset your Sarhne password",
                body),
            cancellationToken);

        return Result.Success();
    }
}
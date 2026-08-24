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

namespace Sarhne.Application.Features.Authentication.ResendConfirmEmail;

public sealed class ResendConfirmEmailCommandHandler
    : IRequestHandler<ResendConfirmEmailCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly ApplicationSettings _applicationSettings;

    public ResendConfirmEmailCommandHandler(
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
        ResendConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Success();

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

        var confirmationLink =
            $"{_applicationSettings.FrontendUrl}" +
            $"/confirm-email" +
            $"?email={Uri.EscapeDataString(user.Email!)}" +
            $"&token={Uri.EscapeDataString(encodedToken)}";

        var body = await _templateRenderer.RenderAsync(
            EmailTemplates.ConfirmEmail,
            new Dictionary<string, string>
            {
                ["FullName"] = user.FullName ?? user.UserName!,
                ["ConfirmationLink"] = confirmationLink,
                //____ devpro _____
                ["Email"] = user.Email!,
                ["ConfirmationToken"] = encodedToken,
                //_________
            },
            cancellationToken);

        await _emailService.SendAsync(
            new EmailRequest(
                user.Email!,
                "Confirm your Sarhne email",
                body),
            cancellationToken);

        return Result.Success();
    }
}
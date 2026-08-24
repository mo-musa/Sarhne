using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Sarhne.Application.Common.Constants;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Common.Settings;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Email;
using Sarhne.Domain.Constants;
using Sarhne.Domain.Entities;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly ApplicationSettings _applicationSettings;
    private readonly ISarhneDbContext _context;
    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IEmailTemplateRenderer templateRenderer,
        IOptions<ApplicationSettings> applicationSettings,
        ISarhneDbContext context)
    {
        _userManager = userManager;
        _emailService = emailService;
        _templateRenderer = templateRenderer;
        _applicationSettings = applicationSettings.Value;
        _context = context;
    }

    public async Task<Result> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var userByEmail = await _userManager.FindByEmailAsync(request.Email);

        if (userByEmail is not null)
        {
            return AuthenticationErrors.EmailAlreadyExists;
        }

        var userByUserName = await _userManager.FindByNameAsync(request.UserName);

        if (userByUserName is not null)
        {
            return AuthenticationErrors.UsernameAlreadyExists;
        }
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var user = new ApplicationUser(request.UserName, request.Email)
            {
                FullName = request.FullName,
                Gender = request.Gender
            };
            // Create User
            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (!identityResult.Succeeded)
            {
                return identityResult.Errors
                    .Select(x => new Error(
                        x.Code,
                        x.Description,
                        ErrorType.Validation))
                    .ToList();
            }

            // Create User Settings
            _context.UserSettings.Add(new UserSetting
            {
                UserId = user.Id
            });
            await _context.SaveChangesAsync(cancellationToken);

            // Add User Role
            var addToRoleResult = await _userManager.AddToRoleAsync(user, Roles.User);

            if (!addToRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return AuthenticationErrors.RoleAssignmentFailed;
            }


            await transaction.CommitAsync(cancellationToken);

            // Generate Confirmation Token
            var token = await _userManager
                .GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            // Confirmation Link
            var confirmationLink =
                $"{_applicationSettings.FrontendUrl}" +
                $"/confirm-email" +
                $"?email={Uri.EscapeDataString(user.Email!)}" +
                $"&token={Uri.EscapeDataString(encodedToken)}";

            // Email Body
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

            // Send Email
            await _emailService.SendAsync(
                new EmailRequest(
                    user.Email!,
                    "Confirm your Sarhne email",
                    body),
                cancellationToken);

            return Result.Success();
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}

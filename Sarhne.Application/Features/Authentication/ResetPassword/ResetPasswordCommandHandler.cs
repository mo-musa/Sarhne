using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.Cookies;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Domain.Constants;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.ResetPassword;

public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICookieService _cookieService;
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        ICookieService cookieService,
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _cookieService = cookieService;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(
            request.Email);

        if (user is null)
        {
            return AuthenticationErrors.InvalidPasswordResetToken;
        }

        string decodedToken;

        try
        {
            decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(request.Token));
        }
        catch
        {
            return AuthenticationErrors.InvalidPasswordResetToken;
        }

        var result = await _userManager.ResetPasswordAsync(
            user,
            decodedToken,
            request.NewPassword);

        if (!result.Succeeded)
        {
            return AuthenticationErrors.InvalidPasswordResetToken;
        }

        var refreshTokens = await _context.RefreshTokens
    .Where(x => x.UserId == user.Id && x.IsActive)
    .ToListAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.Revoke(
                null,
                _currentUserService.IpAddress,
                RefreshTokenReasons.PasswordReset);
        }

        _cookieService.DeleteRefreshTokenCookie();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
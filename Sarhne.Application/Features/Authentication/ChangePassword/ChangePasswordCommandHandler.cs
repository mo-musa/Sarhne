using MediatR;
using Microsoft.AspNetCore.Identity;
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

namespace Sarhne.Application.Features.Authentication.ChangePassword;

public sealed class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICookieService _cookieService;

    public ChangePasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        ICookieService cookieService)
    {
        _userManager = userManager;
        _context = context;
        _currentUserService = currentUserService;
        _cookieService = cookieService;
    }

    public async Task<Result> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated ||
            _currentUserService.UserId is null)
        {
            return AuthenticationErrors.Unauthorized;
        }

        var user = await _userManager.FindByIdAsync(
            _currentUserService.UserId.Value.ToString());

        if (user is null)
        {
            return AuthenticationErrors.Unauthorized;
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            return result.Errors
                .Select(error => new Error(
                    error.Code,
                    error.Description,
                    ErrorType.Validation))
                .ToList();
        }

        var refreshTokens = await _context.RefreshTokens
            .Where(x =>
                x.UserId == user.Id &&
                x.RevokedOnUtc == null &&
                x.ExpiresOnUtc > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.Revoke(
                null,
                _currentUserService.IpAddress,
                RefreshTokenReasons.PasswordChanged);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _cookieService.DeleteRefreshTokenCookie();

        return Result.Success();
    }
}
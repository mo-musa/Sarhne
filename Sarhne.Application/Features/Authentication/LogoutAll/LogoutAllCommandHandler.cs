using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.Cookies;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.LogoutAll;

public sealed class LogoutAllCommandHandler
    : IRequestHandler<LogoutAllCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICookieService _cookieService;
    private readonly ICurrentUserService _currentUserService;

    public LogoutAllCommandHandler(
        ISarhneDbContext context,
        ICookieService cookieService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _cookieService = cookieService;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        LogoutAllCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            return AuthenticationErrors.Unauthorized;
        }
        /* cann't convert IsActive, so we do it manually
        var refreshTokens = await _context.RefreshTokens
            .Where(x =>
                x.UserId == _currentUserService.UserId &&
                x.IsActive)
            .ToListAsync(cancellationToken);
        */
        var refreshTokens = await _context.RefreshTokens
        .Where(x =>
            x.UserId == _currentUserService.UserId &&
            x.RevokedOnUtc == null &&
            x.ExpiresOnUtc > DateTime.UtcNow)
        .ToListAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.Revoke(
                replacedByToken: null,
                ipAddress: _currentUserService.IpAddress,
                reason: RefreshTokenReasons.LoggedOutFromAllDevices);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _cookieService.DeleteRefreshTokenCookie();

        return Result.Success();
    }
}
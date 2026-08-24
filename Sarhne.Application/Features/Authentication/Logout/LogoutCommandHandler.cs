using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.Cookies;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Logout;

public sealed class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICookieService _cookieService;
    private readonly ICurrentUserService _currentUserService;

    public LogoutCommandHandler(
        ISarhneDbContext context,
        ICookieService cookieService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _cookieService = cookieService;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var token = _cookieService.GetRefreshTokenCookie();

        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Success();
        }

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.Token == token,
                cancellationToken);

        if (refreshToken is null)
        {
            _cookieService.DeleteRefreshTokenCookie();

            return Result.Success();
        }

        if (refreshToken.IsActive)
        {
            refreshToken.Revoke(
                replacedByToken: null,
                ipAddress: _currentUserService.IpAddress,
                reason: RefreshTokenReasons.LoggedOut);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _cookieService.DeleteRefreshTokenCookie();

        return Result.Success();
    }
}
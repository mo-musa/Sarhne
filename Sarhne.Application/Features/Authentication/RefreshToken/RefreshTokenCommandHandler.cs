using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.Cookies;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Authentication.Token;
using Sarhne.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.RefreshToken;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly ISarhneDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly ICurrentUserService _currentUserService;

    public RefreshTokenCommandHandler(
        ISarhneDbContext context,
        ITokenService tokenService,
        ICookieService cookieService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _tokenService = tokenService;
        _cookieService = cookieService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var token = _cookieService.GetRefreshTokenCookie();

        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticationErrors.InvalidRefreshToken;
        }

        var refreshToken = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Token == token,
                cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive)
        {
            return AuthenticationErrors.InvalidRefreshToken;
        }
        var accessToken = await _tokenService.GenerateAccessTokenAsync(refreshToken.User);

        var newRefreshToken = _tokenService.GenerateRefreshToken(
        refreshToken.User,
        _currentUserService.IpAddress);

        refreshToken.Revoke(
        newRefreshToken.Token,
        _currentUserService.IpAddress,
        RefreshTokenReasons.Rotated);

        await _context.RefreshTokens.AddAsync(
            newRefreshToken,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(
        accessToken.Token,
        accessToken.ExpiresOnUtc,
        newRefreshToken.Token,
        newRefreshToken.ExpiresOnUtc);
        }
}

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Authentication.Token;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUserService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        ApplicationUser? user;

        if (request.EmailOrUserName.Contains('@'))
        {
            user = await _userManager.FindByEmailAsync(request.EmailOrUserName);
        }
        else
        {
            user = await _userManager.FindByNameAsync(request.EmailOrUserName);
        }

        if (user is null)
        {
            return AuthenticationErrors.InvalidCredentials;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            return AuthenticationErrors.InvalidCredentials;
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            return AuthenticationErrors.EmailNotConfirmed;
        }

        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);

        var refreshToken = _tokenService.GenerateRefreshToken(user, _currentUserService.IpAddress);

        user.RefreshTokens.Add(refreshToken);

        user.LastSeen = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return updateResult.Errors
                .Select(error => new Error(
                    error.Code,
                    error.Description,
                    ErrorType.Unexpected))
                .ToList();
        }

        return new LoginResponse(
                accessToken.Token,
                accessToken.ExpiresOnUtc,
                refreshToken.Token,
                refreshToken.ExpiresOnUtc);
    }
}

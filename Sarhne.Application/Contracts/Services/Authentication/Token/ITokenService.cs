using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Authentication.Token;

public sealed record AccessTokenResult(
string Token,
DateTime ExpiresOnUtc);
public interface ITokenService
{
    Task<AccessTokenResult> GenerateAccessTokenAsync(ApplicationUser user);

    RefreshToken GenerateRefreshToken(
    ApplicationUser user,
    string? createdByIp);
}

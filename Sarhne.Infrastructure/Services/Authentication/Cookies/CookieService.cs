using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sarhne.Application.Contracts.Services.Authentication.Cookies;
using Sarhne.Domain.Constants;
using Sarhne.Infrastructure.Settings;
using System.Runtime;

namespace Sarhne.Infrastructure.Services.Authentication.Cookies;

public sealed class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CookieSettings _options;

    public CookieService(IHttpContextAccessor httpContextAccessor, IOptions<CookieSettings> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public void SetRefreshTokenCookie(string refreshToken,DateTime expiresOnUtc)
    {
        var options = new CookieOptions
        {
            HttpOnly = _options.HttpOnly,
            Secure = _options.Secure,
            SameSite = _options.SameSite,
            Path = _options.Path,
            Domain = _options.Domain,
            Expires = expiresOnUtc
        };

        _httpContextAccessor.HttpContext!
            .Response
            .Cookies
            .Append(
                CookieNames.RefreshToken,
                refreshToken,
                options);
    }

    public void DeleteRefreshTokenCookie()
    {
        _httpContextAccessor.HttpContext?
        .Response
        .Cookies
        .Delete(
            CookieNames.RefreshToken,
            BuildCookieOptions(DateTime.UtcNow.AddDays(-1)));
    }
    public string GetRefreshTokenCookie()
    {
        return _httpContextAccessor.HttpContext?
            .Request
            .Cookies[CookieNames.RefreshToken]!;
    }

    private CookieOptions BuildCookieOptions(DateTime expiresOnUtc)
    {
        return new CookieOptions
        {
            HttpOnly = _options.HttpOnly,
            Secure = _options.Secure,
            SameSite = _options.SameSite,
            Expires = expiresOnUtc.ToUniversalTime()
        };
    }
}

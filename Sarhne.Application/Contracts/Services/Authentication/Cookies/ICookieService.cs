namespace Sarhne.Application.Contracts.Services.Authentication.Cookies;

public interface ICookieService
{
    void SetRefreshTokenCookie(string refreshToken, DateTime expiresOnUtc);

    void DeleteRefreshTokenCookie();
    string GetRefreshTokenCookie();

}
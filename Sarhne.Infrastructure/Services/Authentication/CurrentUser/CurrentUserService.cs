using Microsoft.AspNetCore.Http;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sarhne.Infrastructure.Services.Authentication.CurrentUser;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;
    public int? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var id)
                ? id
                : null;
        }
    }
    public string? UserName =>
        User?
        .FindFirstValue(JwtRegisteredClaimNames.UniqueName);

    public string? Email =>
         User?
        .FindFirstValue(JwtRegisteredClaimNames.Email);

    public string? IpAddress =>
        _httpContextAccessor.HttpContext?
        .Connection
        .RemoteIpAddress?
        .ToString();

    public bool IsAuthenticated =>
     User?
    .Identity?
    .IsAuthenticated == true;

    public IReadOnlyCollection<string> Roles =>
     User?
    .FindAll(ClaimTypes.Role)
    .Select(c => c.Value)
    .ToList()?? [];

    public bool IsInRole(string role)
    {
        return User?.IsInRole(role) == true;
    }
}

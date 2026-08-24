using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Authentication.CurrentUser;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    string? IpAddress { get; }
    public bool IsAuthenticated { get; }
    IReadOnlyCollection<string> Roles { get; }
    bool IsInRole(string role);
}

using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Sarhne.API.Hubs;

public sealed class SarhneUserIdProvider
    : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
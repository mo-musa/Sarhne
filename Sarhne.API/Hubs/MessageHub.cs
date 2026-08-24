using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sarhne.API.Hubs;

[Authorize]
public sealed class MessageHub : Hub
{
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sarhne.API.Hubs;

[Authorize]
public class NotificationHub :Hub
{

}

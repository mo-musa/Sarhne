using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sarhne.API.Extensions;
using Sarhne.Application.Features.User.Notifications.GetNotifications;
using Sarhne.Application.Features.User.Notifications.GetUnreadNotificationsCount;
using Sarhne.Application.Features.User.Notifications.MarkAllNotificationsAsRead;
using Sarhne.Application.Features.User.Notifications.MarkNotificationAsRead;

namespace Sarhne.API.Controllers.User;

[Route("api/User")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetNotificationsQuery(pageNumber, pageSize),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpGet("notifications/unread-count")]
    public async Task<IActionResult> GetUnreadNotificationsCount(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUnreadNotificationsCountQuery(),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpPatch("notifications/{notificationId:int}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(
    int notificationId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MarkNotificationAsReadCommand(notificationId),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpPatch("notifications/read-all")]
    public async Task<IActionResult> MarkAllNotificationsAsRead(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MarkAllNotificationsAsReadCommand(),
            cancellationToken);

        return this.Handle(result);
    }
}

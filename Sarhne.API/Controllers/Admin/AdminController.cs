using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sarhne.API.Authorization;
using Sarhne.API.Extensions;
using Sarhne.Application.Features.Admin.Notifications.SendNotificationToAllUsers;
using Sarhne.Application.Features.Admin.Notifications.SendNotificationToUser;
using Sarhne.Application.Features.Admin.Users.DeleteUser;
using Sarhne.Application.Features.Admin.Users.GetAllUsers;

namespace Sarhne.API.Controllers.Admin;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policies.Admin)]
public class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender)
    {
        _sender = sender;
    }
    //_________________________ Users _________________________

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] GetAllUsersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            query,
            cancellationToken);

        return this.Handle(result);
    }

    [HttpDelete("users/{userId:int}")]
    public async Task<IActionResult> DeleteUser(
        int userId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeleteUserCommand(userId),
            cancellationToken);

        return this.Handle(result);
    }
    //_________________________ Notifications _________________________

    [HttpPost("notifications/users/{userId:int}")]
    public async Task<IActionResult> SendNotificationToUser(
    int userId,
    [FromBody] SendNotificationToUser command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SendNotificationToUserCommand(
                userId,
                command.title,
                command.body),
            cancellationToken);

        return this.Handle(result);
    }
    [HttpPost("notifications/all")]
    public async Task<IActionResult> SendNotificationToAllUsers(
    [FromBody] SendNotificationToAllUsers command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SendNotificationToAllUsersCommand(
                command.title,
                command.body),
            cancellationToken);

        return this.Handle(result);
    }
}

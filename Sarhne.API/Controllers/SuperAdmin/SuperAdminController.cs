using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sarhne.API.Authorization;
using Sarhne.API.Extensions;
using Sarhne.Application.Features.SuperAdmin.Role.AddRoleToUser;
using Sarhne.Application.Features.SuperAdmin.Role.GetAdmins;
using Sarhne.Application.Features.SuperAdmin.Role.RemoveRoleFromUser;

namespace Sarhne.API.Controllers.SuperAdmin;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policies.SuperAdmin)]
public class SuperAdminController : ControllerBase
{
    private readonly ISender _sender;

    public SuperAdminController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("users/{userId:int}/roles/{role}")]
    public async Task<IActionResult> AddRoleToUser(
        int userId,
        string role,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AddRoleToUserCommand(userId, role),
            cancellationToken);

        return this.Handle(result);
    }

    [HttpDelete("users/{userId:int}/roles/{role}")]
    public async Task<IActionResult> RemoveRoleFromUser(
    int userId,
    string role,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RemoveRoleFromUserCommand(userId, role),
            cancellationToken);

        return this.Handle(result);
    }

    [HttpGet("admins")]
    public async Task<IActionResult> GetAdmins(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAdminsQuery(),
            cancellationToken);

        return this.Handle(result);
    }
}

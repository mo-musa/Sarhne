using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sarhne.API.Extensions;
using Sarhne.Application.Features.User.Users.GetCurrentUser;
using Sarhne.Application.Features.User.Users.GetUserByUsername;
using Sarhne.Application.Features.User.Users.UpdateProfileImage;
using Sarhne.Application.Features.User.Users.UpdateUser;

namespace Sarhne.API.Controllers.User;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ISender _sender;

    public UserController(ISender sender)
    {
        _sender = sender;
    }
    [AllowAnonymous]
    [HttpGet("{userName}")]
    public async Task<IActionResult> GetUserByUsername(
        string userName,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUserByUsernameQuery(userName),
            cancellationToken);

        return this.Handle(result);
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCurrentUserQuery(),
            cancellationToken);

        return this.Handle(result);
    }
    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateUser(
    UpdateUserCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }
    [Authorize]
    [HttpPut("me/image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProfileImage(
    [FromForm] UpdateProfileImageCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sarhne.API.Extensions;
using Sarhne.Application.Features.User.Followings.FollowUser;
using Sarhne.Application.Features.User.Followings.GetFollowers;
using Sarhne.Application.Features.User.Followings.GetFollowing;
using Sarhne.Application.Features.User.Followings.UnfollowUser;

namespace Sarhne.API.Controllers.User;

[Route("api/User")]
[ApiController]
public class FollowingController : ControllerBase
{
    private readonly ISender _sender;

    public FollowingController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpPost("{username}/follow")]
    public async Task<IActionResult> FollowUser(
    string username,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new FollowUserCommand(username),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpDelete("{username}/follow")]
    public async Task<IActionResult> UnfollowUser(
    string username,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UnfollowUserCommand(username),
            cancellationToken);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpGet("{username}/followers")]
    public async Task<IActionResult> GetFollowers(
    string username,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetFollowersQuery(
                username,
                pageNumber,
                pageSize),
            cancellationToken);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpGet("{username}/following")]
    public async Task<IActionResult> GetFollowing(
    string username,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetFollowingQuery(
                username,
                pageNumber,
                pageSize),
            cancellationToken);

        return this.Handle(result);
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sarhne.API.Extensions;
using Sarhne.Application.Features.User.UserSettings.GetUserSettings;
using Sarhne.Application.Features.User.UserSettings.UpdateUserSettings;

namespace Sarhne.API.Controllers.User;

[Route("api/User")]
[ApiController]
public class UserSettingController : ControllerBase
{
    private readonly ISender _sender;

    public UserSettingController(ISender sender)
    {
        _sender = sender;
    }
    [Authorize]
    [HttpPut("me/settings")]
    public async Task<IActionResult> UpdateUserSettings(
    UpdateUserSettingsCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpGet("me/settings")]
    public async Task<IActionResult> GetUserSettings(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUserSettingsQuery(),
            cancellationToken);

        return this.Handle(result);
    }
}

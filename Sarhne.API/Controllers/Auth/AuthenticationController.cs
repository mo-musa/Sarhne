using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sarhne.API.Extensions;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Services.Authentication.Cookies;
using Sarhne.Application.Contracts.Services.Email;
using Sarhne.Application.Features.Authentication.ChangePassword;
using Sarhne.Application.Features.Authentication.ConfirmEmail;
using Sarhne.Application.Features.Authentication.ForgotPassword;
using Sarhne.Application.Features.Authentication.Login;
using Sarhne.Application.Features.Authentication.Logout;
using Sarhne.Application.Features.Authentication.LogoutAll;
using Sarhne.Application.Features.Authentication.Me;
using Sarhne.Application.Features.Authentication.RefreshToken;
using Sarhne.Application.Features.Authentication.Register;
using Sarhne.Application.Features.Authentication.ResendConfirmEmail;
using Sarhne.Application.Features.Authentication.ResetPassword;
namespace Sarhne.API.Controllers.Auth;

[EnableRateLimiting("fixed")]
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICookieService _cookieService;

    public AuthenticationController(ISender sender, ICookieService cookieService)
    {
        _sender = sender;
        _cookieService = cookieService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
    ConfirmEmailCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail(
    ResendConfirmEmailCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command,CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return this.Handle(result);

        _cookieService.SetRefreshTokenCookie(
             result.Value.RefreshToken,
             result.Value.RefreshTokenExpiresOnUtc);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RefreshTokenCommand(),
            cancellationToken);

        if (!result.IsSuccess)
            return this.Handle(result);

        _cookieService.SetRefreshTokenCookie(
            result.Value.RefreshToken,
            result.Value.RefreshTokenExpiresOnUtc);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new LogoutCommand(),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new LogoutAllCommand(),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MeQuery(),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
    ChangePasswordCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    ForgotPasswordCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
    ResetPasswordCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }
}

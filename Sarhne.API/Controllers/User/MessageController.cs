using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sarhne.API.Extensions;
using Sarhne.Application.Features.User.Messages.AddLikeToMessage;
using Sarhne.Application.Features.User.Messages.EditMessage;
using Sarhne.Application.Features.User.Messages.GetMessageById;
using Sarhne.Application.Features.User.Messages.GetMessagesByMe;
using Sarhne.Application.Features.User.Messages.GetMessagesToMe;
using Sarhne.Application.Features.User.Messages.GetPublicMessagesByUser;
using Sarhne.Application.Features.User.Messages.GetUnreadMessagesCount;
using Sarhne.Application.Features.User.Messages.MarkMessageAsRead;
using Sarhne.Application.Features.User.Messages.SendMessage;
using Sarhne.Application.Features.User.Messages.SetMessageHidden;
using Sarhne.Application.Features.User.Messages.ToggleStarMessage;

namespace Sarhne.API.Controllers.User;

[Route("api/User")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly ISender _sender;

    public MessageController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpPost("message")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendMessage(
    [FromForm] SendMessageCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpGet("messages/to-me")]
    public async Task<IActionResult> GetMessagesToMe(
    [FromQuery] GetMessagesToMeQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            query,
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpGet("messages/by-me")]
    public async Task<IActionResult> GetMessagesByMe(
    [FromQuery] GetMessagesByMeQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            query,
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpGet("messages/{messageId:int}")]
    public async Task<IActionResult> GetMessageById(
    int messageId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetMessageByIdQuery(messageId),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpPatch("messages/{messageId:int}/read")]
    public async Task<IActionResult> MarkMessageAsRead(
    int messageId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MarkMessageAsReadCommand(messageId),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpPatch("messages/{messageId:int}/star")]
    public async Task<IActionResult> ToggleStarMessage(
    int messageId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ToggleStarMessageCommand(messageId),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpGet("messages/unread-count")]
    public async Task<IActionResult> GetUnreadMessagesCount(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUnreadMessagesCountQuery(),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpPatch("messages/{messageId:int}/hidden")]
    public async Task<IActionResult> SetMessageHidden(
        int messageId,
        [FromBody] SetMessageHiddenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SetMessageHiddenCommand(
                messageId,
                request.IsHidden),
            cancellationToken);

        return this.Handle(result);
    }

    [HttpGet("users/{userId:int}/messages")]
    public async Task<IActionResult> GetPublicMessagesByUser(
    int userId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetPublicMessagesByUserQuery(
                userId,
                pageNumber,
                pageSize),
            cancellationToken);

        return this.Handle(result);
    }

    [HttpPatch("messages/{messageId:int}/like")]
    public async Task<IActionResult> AddLikeToMessage(
    int messageId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AddLikeToMessageCommand(messageId),
            cancellationToken);

        return this.Handle(result);
    }

    [Authorize]
    [HttpPatch("messages/edit")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> EditMessage(
    [FromForm] EditMessageCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return this.Handle(result);
    }
}

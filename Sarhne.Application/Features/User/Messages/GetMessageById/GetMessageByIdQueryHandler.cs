using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Features.User.Messages.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetMessageById;

public sealed class GetMessageByIdQueryHandler
    : IRequestHandler<
        GetMessageByIdQuery,
        Result<MessageResponse>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMessageByIdQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<MessageResponse>> Handle(
        GetMessageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var message = await _context.Messages
            .AsNoTracking()
            .Where(x =>
                x.Id == request.MessageId &&
                (
                    x.SenderId == currentUserId.Value ||
                    x.ReceiverId == currentUserId.Value
                ))
            .Select(x => new
            {
                x.Id,
                x.Content,
                x.PhotoUrl,
                x.IsAnonymous,
                x.IsRead,
                x.IsStarred,
                x.LikesCount,
                x.CreatedAt,

                x.SenderId,
                SenderUserName = x.Sender.UserName,
                SenderImageUrl = x.Sender.ImageUrl,

                x.ReceiverId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (message is null)
            return MessageErrors.MessageNotFound;

        var isSender =
            message.SenderId == currentUserId.Value;

        var response = new MessageResponse(
            message.Id,
            message.Content,
            message.PhotoUrl,
            message.IsAnonymous,
            message.IsRead,
            message.IsStarred,
            message.LikesCount,
            message.CreatedAt,

            // Anonymous only hides sender from receiver.
            message.IsAnonymous && !isSender
                ? null
                : message.SenderId,

            message.IsAnonymous && !isSender
                ? null
                : message.SenderUserName,

            message.IsAnonymous && !isSender
                ? null
                : message.SenderImageUrl
        );

        return response;
    }
}
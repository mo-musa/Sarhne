using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.MarkMessageAsRead;

public sealed class MarkMessageAsReadCommandHandler
    : IRequestHandler<
        MarkMessageAsReadCommand,
        Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessageRealtimeService _messageRealtimeService;

    public MarkMessageAsReadCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        IMessageRealtimeService messageRealtimeService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _messageRealtimeService = messageRealtimeService;
    }

    public async Task<Result> Handle(
        MarkMessageAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var message = await _context.Messages
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.MessageId &&
                    x.ReceiverId == currentUserId.Value,
                cancellationToken);

        if (message is null)
            return MessageErrors.MessageNotFound;

        if (message.IsRead)
            return Result.Success();

        message.MarkAsRead();

        await _context.SaveChangesAsync(cancellationToken);

        await _messageRealtimeService.SendMessageReadAsync(
            message.SenderId,
            message.Id,
            cancellationToken);

        return Result.Success();
    }
}
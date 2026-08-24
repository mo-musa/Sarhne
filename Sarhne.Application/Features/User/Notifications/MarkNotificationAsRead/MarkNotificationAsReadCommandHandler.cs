using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Dtos;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Notifications.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler
    : IRequestHandler<
        MarkNotificationAsReadCommand,
        Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRealtimeService _notificationRealtimeService;

    public MarkNotificationAsReadCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        INotificationRealtimeService notificationRealtimeService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationRealtimeService = notificationRealtimeService;
    }

    public async Task<Result> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.NotificationId &&
                    x.ReceiverId == currentUserId,
                cancellationToken);

        if (notification is null)
            return NotificationErrors.NotificationNotFound;

        if (!notification.IsRead)
        {
            notification.MarkAsRead();

            await _context.SaveChangesAsync(cancellationToken);

        await _notificationRealtimeService.SendNotificationReadAsync(
            currentUserId.Value,
            new NotificationReadDto
            {
                NotificationId = notification.Id
            },
            cancellationToken);
        }

        return Result.Success();
    }
}
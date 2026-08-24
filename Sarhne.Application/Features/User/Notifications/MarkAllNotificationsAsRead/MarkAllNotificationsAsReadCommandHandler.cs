using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Notifications.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler
    : IRequestHandler<
        MarkAllNotificationsAsReadCommand,
        Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRealtimeService _notificationRealtimeService;

    public MarkAllNotificationsAsReadCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        INotificationRealtimeService notificationRealtimeService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationRealtimeService = notificationRealtimeService;
    }

    public async Task<Result> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var notifications = await _context.Notifications
            .Where(x =>
                x.ReceiverId == currentUserId &&
                !x.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.MarkAsRead();
        }

        if (notifications.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        await _notificationRealtimeService.SendAllNotificationsReadAsync(
            currentUserId.Value,
            cancellationToken);

        return Result.Success();
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Dtos;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Notifications.SendNotificationToUser;

public sealed class SendNotificationToUserCommandHandler
    : IRequestHandler<SendNotificationToUserCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly INotificationRealtimeService _notificationRealtimeService;

    public SendNotificationToUserCommandHandler(
        ISarhneDbContext context,
        INotificationService notificationService,
        INotificationRealtimeService notificationRealtimeService)
    {
        _context = context;
        _notificationService = notificationService;
        _notificationRealtimeService = notificationRealtimeService;
    }

    public async Task<Result> Handle(
        SendNotificationToUserCommand request,
        CancellationToken cancellationToken)
    {
        var userExists = await _context.Users
            .AnyAsync(
                x => x.Id == request.UserId,
                cancellationToken);

        if (!userExists)
            return UserErrors.UserNotFound;

        var notification =
            await _notificationService.CreateSystemNotificationAsync(
                request.UserId,
                request.Title,
                request.Body,
                cancellationToken);

        if (notification is null)
            return Result.Success();

        await _context.SaveChangesAsync(cancellationToken);

        var realtimeNotification =
            await _context.Notifications
                .AsNoTracking()
                .Where(x => x.Id == notification.Id)
                .Select(x => new NotificationRealtimeDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    Title = x.Title,
                    Body = x.Body,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt,
                    SenderId = null,
                    SenderUserName = null,
                    SenderImageUrl = null
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (realtimeNotification is not null)
        {
            await _notificationRealtimeService.SendNotificationAsync(
                request.UserId,
                realtimeNotification,
                cancellationToken);
        }

        return Result.Success();
    }
}
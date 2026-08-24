using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Dtos;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Notifications;
using Sarhne.Domain.Constants;
using Sarhne.Domain.Entities;
using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Notifications.SendNotificationToAllUsers;

public sealed class SendNotificationToAllUsersCommandHandler
    : IRequestHandler<SendNotificationToAllUsersCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly INotificationRealtimeService _notificationRealtimeService;

    public SendNotificationToAllUsersCommandHandler(
        ISarhneDbContext context,
        INotificationService notificationService,
        INotificationRealtimeService notificationRealtimeService)
    {
        _context = context;
        _notificationService = notificationService;
        _notificationRealtimeService = notificationRealtimeService;
    }

    public async Task<Result> Handle(
        SendNotificationToAllUsersCommand request,
        CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Where(x =>
            x.UserSetting.AllowReceiveNotifications &&
            x.UserRoles.Any(ur=>ur.Role.Name == Roles.User))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var notificationIds = new List<Notification>();

        foreach (var userId in users)
        {
            var notification =
                await _notificationService.CreateSystemNotificationAsync(
                    userId,
                    request.Title,
                    request.Body,
                    cancellationToken);

            if (notification is not null)
            {
                notificationIds.Add(notification);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (notificationIds.Count == 0)
            return Result.Success();

        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(x => notificationIds.Contains(x))
            .Select(x => new
            {
                x.ReceiverId,
                Dto = new NotificationRealtimeDto
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
                }
            })
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            await _notificationRealtimeService.SendNotificationAsync(
                notification.ReceiverId,
                notification.Dto,
                cancellationToken);
        }

        return Result.Success();
    }
}
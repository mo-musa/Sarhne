using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sarhne.Application.Common.Caching;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Caching;
using Sarhne.Application.Contracts.Services.Notifications;
using Sarhne.Domain.Entities;
using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sarhne.Infrastructure.Services.Notifications;

public sealed class NotificationService
    : INotificationService
{
    private readonly ISarhneDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ISarhneDbContext context,
        ICacheService cacheService,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Notification?> CreateFollowNotificationAsync(
        int senderId,
        int receiverId,
        CancellationToken cancellationToken = default)
    {
        if (!await CanReceiveNotificationsAsync(receiverId, cancellationToken))
            return null;

        var senderName = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == senderId)
            .Select(x => x.FullName)
            .FirstOrDefaultAsync(cancellationToken);

        if (senderName is null)
            return null;

        var notification = new Notification(
            NotificationType.Follow,
            "New follower",
            $"{senderName} started following you.",
            senderId,
            receiverId);

        _context.Notifications.Add(notification);

        return notification;
    }

    public async Task<Notification?> CreateMessageNotificationAsync(
        int senderId,
        int receiverId,
        CancellationToken cancellationToken = default)
    {
        if (!await CanReceiveNotificationsAsync(receiverId, cancellationToken))
            return null;

        var notification = new Notification(
            NotificationType.NewMessage,
            "New message",
            "You have received a new message.",
            senderId,
            receiverId);

        _context.Notifications.Add(notification);

        return notification;
    }

    public async Task<Notification?> CreateSystemNotificationAsync(
        int receiverId,
        string title,
        string body,
        CancellationToken cancellationToken = default)
    {
        if (!await CanReceiveNotificationsAsync(receiverId, cancellationToken))
            return null;

        var notification = new Notification(
            NotificationType.System,
            title,
            body,
            null,
            receiverId);

        _context.Notifications.Add(notification);

        return notification;
    }
    private async Task<bool> CanReceiveNotificationsAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.UserSettings(userId);

        var cachedUserSetting =
            await _cacheService.GetAsync<UserSetting>(cacheKey);

        if (cachedUserSetting is not null)
            return cachedUserSetting.AllowReceiveNotifications;

        var userSetting = await _context.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (userSetting is null)
            return false;

        await _cacheService.SetAsync(
            cacheKey,
            userSetting,
            TimeSpan.FromMinutes(15));

        return userSetting.AllowReceiveNotifications;
    }
}
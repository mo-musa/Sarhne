using Sarhne.Domain.Entities;
using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Notifications;

public interface INotificationService
{
    Task<Notification?> CreateFollowNotificationAsync(
        int senderId,
        int receiverId,
        CancellationToken cancellationToken = default);

    Task<Notification?> CreateMessageNotificationAsync(
    int senderId,
    int receiverId,
    CancellationToken cancellationToken = default);

    Task<Notification?> CreateSystemNotificationAsync(
    int receiverId,
    string title,
    string body,
    CancellationToken cancellationToken = default);
}
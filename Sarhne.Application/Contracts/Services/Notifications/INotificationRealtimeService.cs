using Sarhne.Domain.Enums;
using Sarhne.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Notifications;

public interface INotificationRealtimeService
{
    Task SendNotificationAsync(
        int receiverId,
        NotificationRealtimeDto notification,
        CancellationToken cancellationToken = default);

    Task SendNotificationReadAsync(
        int receiverId,
        NotificationReadDto notification,
        CancellationToken cancellationToken = default);

    Task SendAllNotificationsReadAsync(
        int receiverId,
        CancellationToken cancellationToken = default);
}

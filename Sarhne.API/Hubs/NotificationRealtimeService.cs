using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Contracts.Services.Notifications;
using Sarhne.Application.Common.Dtos;

namespace Sarhne.API.Hubs;

public sealed class NotificationRealtimeService
    : INotificationRealtimeService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationRealtimeService(
        IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(
        int receiverId,
        NotificationRealtimeDto notification,
        CancellationToken cancellationToken = default)
    {
        await _hubContext
           .Clients
           .User(receiverId.ToString())
           .SendAsync(
               "NotificationReceived",
               notification,
               cancellationToken);
    }

    public async Task SendNotificationReadAsync(
        int receiverId,
        NotificationReadDto notification,
        CancellationToken cancellationToken = default)
    {
        await _hubContext
            .Clients
            .User(receiverId.ToString())
            .SendAsync(
                "NotificationRead",
                notification,
                cancellationToken);
    }

    public async Task SendAllNotificationsReadAsync(
    int receiverId,
    CancellationToken cancellationToken = default)
    {
        await _hubContext
            .Clients
            .User(receiverId.ToString())
            .SendAsync(
                "AllNotificationsRead",
                cancellationToken);
    }
}
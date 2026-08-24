using Microsoft.AspNetCore.SignalR;
using Sarhne.Application.Common.Dtos;
using Sarhne.Application.Contracts.Services.Messages;

namespace Sarhne.API.Hubs;

public sealed class MessageRealtimeService
    : IMessageRealtimeService
{
    private readonly IHubContext<MessageHub> _hubContext;

    public MessageRealtimeService(
        IHubContext<MessageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageAsync(
        int receiverId,
        MessageRealtimeDto message,
        CancellationToken cancellationToken = default)
    {
        await _hubContext
            .Clients
            .User(receiverId.ToString())
            .SendAsync(
                "MessageReceived",
                message,
                cancellationToken);
    }

    public async Task SendMessageReadAsync(
    int senderId,
    int messageId,
    CancellationToken cancellationToken = default)
    {
        await _hubContext
            .Clients
            .User(senderId.ToString())
            .SendAsync(
                "MessageRead",
                new
                {
                    MessageId = messageId
                },
                cancellationToken);
    }
}
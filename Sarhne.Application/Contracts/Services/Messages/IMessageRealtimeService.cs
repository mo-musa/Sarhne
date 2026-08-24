using Sarhne.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Messages;

public interface IMessageRealtimeService
{
    Task SendMessageAsync(
        int receiverId,
        MessageRealtimeDto message,
        CancellationToken cancellationToken = default);
    Task SendMessageReadAsync(
        int senderId,
        int messageId,
        CancellationToken cancellationToken = default);
}
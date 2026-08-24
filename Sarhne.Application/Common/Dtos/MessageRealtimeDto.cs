using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Dtos;

public sealed class MessageRealtimeDto
{
    public int Id { get; init; }

    public string? Content { get; init; }

    public string? PhotoUrl { get; init; }

    public bool IsAnonymous { get; init; }

    public bool IsRead { get; init; }

    public bool IsStarred { get; init; }

    public int LikesCount { get; init; }

    public DateTime CreatedAt { get; init; }

    public int? SenderId { get; init; }

    public string? SenderUserName { get; init; }

    public string? SenderImageUrl { get; init; }
}
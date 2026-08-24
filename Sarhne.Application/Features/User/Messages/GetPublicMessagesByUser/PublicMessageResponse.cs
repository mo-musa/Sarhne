using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetPublicMessagesByUser;

public sealed class PublicMessageResponse
{
    public int Id { get; init; }

    public string? Content { get; init; }

    public string? PhotoUrl { get; init; }

    public bool IsAnonymous { get; init; }

    public int LikesCount { get; init; }

    public DateTime CreatedAt { get; init; }

    public int? SenderId { get; init; }

    public string? SenderUserName { get; init; }

    public string? SenderImageUrl { get; init; }
}
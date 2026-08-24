using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Notifications.GetNotifications;

public sealed class NotificationResponseDto
{
    public int Id { get; set; }

    public NotificationType Type { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? SenderId { get; set; }

    public string? SenderUserName { get; set; }

    public string? SenderImageUrl { get; set; }
}
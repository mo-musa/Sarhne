using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.Common;

public sealed record MessageResponse(
    int Id,
    string? Content,
    string? PhotoUrl,
    bool IsAnonymous,
    bool IsRead,
    bool IsStarred,
    int LikesCount,
    DateTime CreatedAt,
    int? SenderId,
    string? SenderUserName,
    string? SenderImageUrl);
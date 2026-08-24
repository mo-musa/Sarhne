using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Notifications.GetNotifications;

public sealed record GetNotificationsQuery(
    int Page = 1,
    int PageSize = 20)
    : IRequest<Result<PagedResult<NotificationResponseDto>>>;
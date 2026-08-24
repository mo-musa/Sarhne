using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Notifications.SendNotificationToUser;

public sealed record SendNotificationToUserCommand(
    int UserId,
    string Title,
    string Body) : IRequest<Result>;
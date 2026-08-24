using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Notifications.SendNotificationToAllUsers;

public sealed record SendNotificationToAllUsersCommand(
    string Title,
    string Body) : IRequest<Result>;
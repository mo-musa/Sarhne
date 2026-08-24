using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Notifications.GetUnreadNotificationsCount;

public sealed record GetUnreadNotificationsCountQuery
    : IRequest<Result<int>>;
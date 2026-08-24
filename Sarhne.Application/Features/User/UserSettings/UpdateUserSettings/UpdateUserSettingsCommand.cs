using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.UserSettings.UpdateUserSettings;

public sealed record UpdateUserSettingsCommand(
    bool? AllowAnonymousMessages,
    bool? AllowMessages,
    bool? AllowSendPhoto,
    bool? AllowReceiveEmail,
    bool? AllowReceiveNotifications,
    bool? ShowLastSeen,
    bool? ShowProfileViewsCount
) : IRequest<Result>;
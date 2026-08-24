using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.UserSettings.GetUserSettings;

public sealed class UserSettingsResponseDto
{
    public bool AllowAnonymousMessages { get; set; }
    public bool AllowMessages { get; set; }
    public bool AllowSendPhoto { get; set; }
    public bool AllowReceiveEmail { get; set; }
    public bool AllowReceiveNotifications { get; set; }
    public bool ShowLastSeen { get; set; }
    public bool ShowProfileViewsCount { get; set; }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Notifications.SendNotificationToUser;

public sealed record SendNotificationToUser(string title, string body);
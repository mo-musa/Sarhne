using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Errors;

public static class NotificationErrors
{
    public static readonly Error NotificationNotFound =
        new(
            "Notification.NotFound",
            "Notification was not found.",
            ErrorType.NotFound);
}
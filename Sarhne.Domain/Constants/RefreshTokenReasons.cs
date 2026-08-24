using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Domain.Constants;

public static class RefreshTokenReasons
{
    public const string Rotated =
        "Replaced by a new refresh token.";

    public const string LoggedOut =
        "Logged out.";

    public const string LoggedOutFromAllDevices =
        "Logged out from all devices.";

    public const string PasswordChanged =
        "Password changed.";

    public const string PasswordReset =
        "Password had reset.";
}
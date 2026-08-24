using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Login;

public sealed record LoginResponse(
    string AccessToken,
    DateTime AccessTokenExpiresOnUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresOnUtc);
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.RefreshToken;

public sealed record RefreshTokenResponse(
    string AccessToken,
    DateTime AccessTokenExpiresOnUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresOnUtc);
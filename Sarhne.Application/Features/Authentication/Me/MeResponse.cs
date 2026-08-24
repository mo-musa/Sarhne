using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Me;

public sealed record MeResponse(
    int Id,
    string UserName,
    string FullName,
    string Email,
    IEnumerable<string> Roles);
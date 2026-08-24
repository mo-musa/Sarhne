using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Login;

public sealed record LoginCommand(
    string EmailOrUserName,
    string Password)
    : IRequest<Result<LoginResponse>>;

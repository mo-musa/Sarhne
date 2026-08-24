using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.ResendConfirmEmail;

public sealed record ResendConfirmEmailCommand(
    string Email) : IRequest<Result>;
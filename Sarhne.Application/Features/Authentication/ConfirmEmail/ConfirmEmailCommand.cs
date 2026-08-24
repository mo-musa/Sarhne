using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string Email,
    string Token) : IRequest<Result>;
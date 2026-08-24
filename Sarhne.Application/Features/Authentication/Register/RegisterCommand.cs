using MediatR;
using Sarhne.Application.Common.Results;
using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Register;

public sealed record RegisterCommand(
string UserName,
string Email,
string Password,
string FullName,
Gender Gender)
: IRequest<Result>;

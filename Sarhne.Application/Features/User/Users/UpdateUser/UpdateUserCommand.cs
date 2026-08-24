using MediatR;
using Sarhne.Application.Common.Results;
using Sarhne.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.UpdateUser;

public sealed record UpdateUserCommand(
    string? FullName,
    string? AboutMe,
    Gender? Gender
) : IRequest<Result>;
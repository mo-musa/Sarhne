using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.GetCurrentUser;

public sealed record GetCurrentUserQuery
    : IRequest<Result<CurrentUserResponseDto>>;
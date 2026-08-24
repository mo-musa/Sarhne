using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.GetUserByUsername;

public sealed record GetUserByUsernameQuery(
    string UserName
) : IRequest<Result<UserResponseDto>>;

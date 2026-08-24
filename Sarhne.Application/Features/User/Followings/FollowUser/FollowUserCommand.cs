using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.FollowUser;

public sealed record FollowUserCommand(
    string UserName
) : IRequest<Result>;
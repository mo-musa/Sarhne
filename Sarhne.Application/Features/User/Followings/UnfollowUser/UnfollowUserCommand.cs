using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.UnfollowUser;

public sealed record UnfollowUserCommand(
    string UserName
) : IRequest<Result>;
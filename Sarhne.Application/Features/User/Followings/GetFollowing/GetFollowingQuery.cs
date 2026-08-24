using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.GetFollowing;

public sealed record GetFollowingQuery(
    string UserName,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<FollowingUserResponseDto>>>;
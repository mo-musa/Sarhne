using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.GetFollowers;

public sealed record GetFollowersQuery(
    string UserName,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<FollowerResponseDto>>>;
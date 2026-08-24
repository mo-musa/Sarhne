using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Users.GetAllUsers;

public sealed record GetAllUsersQuery(
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<GetAllUsersDto>>>;
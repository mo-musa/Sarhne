using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetPublicMessagesByUser;

public sealed record GetPublicMessagesByUserQuery(
    int UserId,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<Result<PagedResult<PublicMessageResponse>>>;

using MediatR;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Features.User.Messages.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetMessagesToMe;

public sealed record GetMessagesToMeQuery(
    bool? IsRead = null,
    bool? IsStarred = null,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<Result<PagedResult<MessageResponse>>>;

using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetUnreadMessagesCount;

public sealed record GetUnreadMessagesCountQuery
    : IRequest<Result<int>>;
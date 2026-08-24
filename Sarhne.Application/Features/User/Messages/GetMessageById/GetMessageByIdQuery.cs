using MediatR;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Features.User.Messages.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetMessageById;

public sealed record GetMessageByIdQuery(
    int MessageId)
    : IRequest<Result<MessageResponse>>;
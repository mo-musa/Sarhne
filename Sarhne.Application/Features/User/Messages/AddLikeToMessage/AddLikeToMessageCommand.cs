using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.AddLikeToMessage;

public sealed record AddLikeToMessageCommand(
    int MessageId) : IRequest<Result>;

using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.ToggleStarMessage;

public sealed record ToggleStarMessageCommand(
    int MessageId) : IRequest<Result>;
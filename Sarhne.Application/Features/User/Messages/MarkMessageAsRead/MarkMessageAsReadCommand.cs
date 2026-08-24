using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.MarkMessageAsRead;

public sealed record MarkMessageAsReadCommand(
    int MessageId) : IRequest<Result>;
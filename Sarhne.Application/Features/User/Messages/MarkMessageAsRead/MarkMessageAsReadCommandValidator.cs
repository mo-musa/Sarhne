using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.MarkMessageAsRead;

public sealed class MarkMessageAsReadCommandValidator
    : AbstractValidator<MarkMessageAsReadCommand>
{
    public MarkMessageAsReadCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .GreaterThan(0);
    }
}
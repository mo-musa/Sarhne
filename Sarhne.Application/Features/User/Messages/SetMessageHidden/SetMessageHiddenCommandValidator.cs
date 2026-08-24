using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.SetMessageHidden;

public sealed class SetMessageHiddenCommandValidator
    : AbstractValidator<SetMessageHiddenCommand>
{
    public SetMessageHiddenCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .GreaterThan(0);
    }
}
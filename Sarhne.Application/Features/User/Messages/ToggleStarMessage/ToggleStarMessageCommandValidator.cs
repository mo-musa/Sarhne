using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.ToggleStarMessage;

public sealed class ToggleStarMessageCommandValidator
    : AbstractValidator<ToggleStarMessageCommand>
{
    public ToggleStarMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .GreaterThan(0);
    }
}
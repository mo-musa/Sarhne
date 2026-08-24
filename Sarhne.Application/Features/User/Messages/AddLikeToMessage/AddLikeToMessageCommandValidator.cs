using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.AddLikeToMessage;

public sealed class AddLikeToMessageCommandValidator
    : AbstractValidator<AddLikeToMessageCommand>
{
    public AddLikeToMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .GreaterThan(0);
    }
}
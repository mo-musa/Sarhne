using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetMessageById;

public sealed class GetMessageByIdQueryValidator
    : AbstractValidator<GetMessageByIdQuery>
{
    public GetMessageByIdQueryValidator()
    {
        RuleFor(x => x.MessageId)
            .GreaterThan(0);
    }
}
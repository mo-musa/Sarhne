using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetMessagesByMe;

public sealed class GetMessagesByMeQueryValidator
    : AbstractValidator<GetMessagesByMeQuery>
{
    public GetMessagesByMeQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
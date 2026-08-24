using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetMessagesToMe;

public sealed class GetMessagesToMeQueryValidator
    : AbstractValidator<GetMessagesToMeQuery>
{
    public GetMessagesToMeQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.GetFollowing;

public sealed class GetFollowingQueryValidator
    : AbstractValidator<GetFollowingQuery>
{
    public GetFollowingQueryValidator()
    {
        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters.")
            .MaximumLength(30)
            .WithMessage("Username must not exceed 30 characters.")
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage(
                "Username can only contain letters, numbers, and underscores.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");
    }
}
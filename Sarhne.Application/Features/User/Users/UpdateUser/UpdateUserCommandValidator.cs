using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.UpdateUser;

public sealed class UpdateUserCommandValidator
    : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x)
        .Must(x =>
            x.FullName is not null ||
            x.AboutMe is not null ||
            x.Gender.HasValue)
        .WithMessage("At least one field must be provided.");

        RuleFor(x => x.FullName)
            .MaximumLength(100)
            .WithMessage("Full name must not exceed 100 characters.")
            .When(x => x.FullName is not null);

        RuleFor(x => x.AboutMe)
            .MaximumLength(500)
            .WithMessage("About me must not exceed 500 characters.")
            .When(x => x.AboutMe is not null);

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage("Invalid gender.")
            .When(x => x.Gender.HasValue);
    }
}
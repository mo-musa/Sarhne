using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.ResendConfirmEmail;

public sealed class ResendConfirmEmailCommandValidator
    : AbstractValidator<ResendConfirmEmailCommand>
{
    public ResendConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("Invalid email address.");
    }
}
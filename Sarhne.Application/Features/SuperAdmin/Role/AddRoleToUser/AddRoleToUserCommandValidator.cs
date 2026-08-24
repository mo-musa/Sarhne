using FluentValidation;
using Sarhne.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.SuperAdmin.Role.AddRoleToUser;

public sealed class AddRoleToUserCommandValidator
    : AbstractValidator<AddRoleToUserCommand>
{
    public AddRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(x =>
                string.Equals(
                    x,
                    Roles.Admin,
                    StringComparison.OrdinalIgnoreCase))
            .WithMessage(
                $"Only the '{Roles.Admin}' role can be assigned.");
    }
}
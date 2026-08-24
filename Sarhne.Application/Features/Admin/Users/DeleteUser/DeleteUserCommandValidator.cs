using FluentValidation;
using Sarhne.Application.Features.SuperAdmin.Role.AddRoleToUser;
using Sarhne.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Users.DeleteUser;

public sealed class DeleteUserCommandValidator: AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
        .GreaterThan(0);
    }
}

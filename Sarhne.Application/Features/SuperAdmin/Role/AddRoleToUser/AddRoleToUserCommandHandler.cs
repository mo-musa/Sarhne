using MediatR;
using Microsoft.AspNetCore.Identity;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Domain.Constants;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.SuperAdmin.Role.AddRoleToUser;

public sealed class AddRoleToUserCommandHandler
    : IRequestHandler<AddRoleToUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AddRoleToUserCommandHandler(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(
        AddRoleToUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(
            request.UserId.ToString());

        if (user is null)
            return UserErrors.UserNotFound;

        if (await _userManager.IsInRoleAsync(
                user,
                Roles.Admin))
        {
            return RoleErrors.UserAlreadyHasRole;
        }

        var result = await _userManager.AddToRoleAsync(
            user,
            Roles.Admin);

        if (!result.Succeeded)
        {
            return RoleErrors.RoleAssignmentFailed;
        }

        return Result.Success();
    }
}
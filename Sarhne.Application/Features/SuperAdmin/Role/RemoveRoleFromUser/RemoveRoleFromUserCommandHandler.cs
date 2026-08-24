using MediatR;
using Microsoft.AspNetCore.Identity;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.SuperAdmin.Role.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommandHandler
    : IRequestHandler<RemoveRoleFromUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RemoveRoleFromUserCommandHandler(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(
        RemoveRoleFromUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(
            request.UserId.ToString());

        if (user is null)
            return UserErrors.UserNotFound;

        var isInRole = await _userManager.IsInRoleAsync(
            user,
            request.Role);

        if (!isInRole)
            return RoleErrors.UserDoesNotHaveRole;

        var result = await _userManager.RemoveFromRoleAsync(
            user,
            request.Role);

        if (!result.Succeeded)
            return RoleErrors.RoleRemovalFailed;

        return Result.Success();
    }
}
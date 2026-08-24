using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Errors;

public static class RoleErrors
{
    public static readonly Error UserAlreadyHasRole =
        new(
            "Role.UserAlreadyHasRole",
            "User already has this role.",
            ErrorType.Conflict);

    public static readonly Error RoleAssignmentFailed =
        new(
            "Role.RoleAssignmentFailed",
            "Failed to assign role to user.",
            ErrorType.Unexpected);

    public static readonly Error UserDoesNotHaveRole =
        new(
            "Role.UserDoesNotHaveRole",
            "User does not have this role.",
            ErrorType.NotFound);

    public static readonly Error RoleRemovalFailed =
        new(
            "Role.RoleRemovalFailed",
            "Failed to remove role from user.",
            ErrorType.Unexpected);
}
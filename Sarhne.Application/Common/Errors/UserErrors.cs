using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound =
        new(
            "User.NotFound",
            "User was not found.",
            ErrorType.NotFound);
    public static readonly Error CannotFollowYourself =
        new(
            "User.CannotFollowYourself",
            "You cannot follow yourself.",
            ErrorType.Validation);

    public static readonly Error AlreadyFollowing =
        new(
            "User.AlreadyFollowing",
            "You are already following this user.",
            ErrorType.Conflict);

    public static readonly Error NotFollowing =
        new(
            "User.NotFollowing",
            "You are not following this user.",
            ErrorType.Conflict);

    public static readonly Error CannotDeleteAdmin =
        new(
            "User.CannotDeleteAdmin",
            "You cannot delete admin.",
            ErrorType.Forbidden);

    public static readonly Error CannotDeleteYourself =
        new(
            "User.CannotDeleteYourself",
            "You cannot delete Yourself.",
            ErrorType.Validation);

}

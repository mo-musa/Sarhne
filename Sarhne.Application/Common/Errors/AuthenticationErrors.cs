using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Errors;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials =
        new(
            "Auth.InvalidCredentials",
            "Invalid email/username or password.",
            ErrorType.Unauthorized);

    public static readonly Error EmailAlreadyExists =
        new(
            "Auth.EmailAlreadyExists",
            "Email already exists.",
            ErrorType.Conflict);

    public static readonly Error UsernameAlreadyExists =
        new(
            "Auth.UsernameAlreadyExists",
            "Username already exists.",
            ErrorType.Conflict);

    public static readonly Error EmailNotConfirmed =
        new(
            "Auth.EmailNotConfirmed",
            "Please confirm your email address.",
            ErrorType.Validation);

    public static readonly Error RegistrationFailed =
        new(
            "Auth.RegistrationFailed",
            "Registration failed.",
            ErrorType.Unexpected);

    public static readonly Error RoleAssignmentFailed =
        new(
            "Auth.RoleAssignmentFailed",
            "Failed to assign the default role to the user.",
            ErrorType.Unexpected);

    public static readonly Error InvalidRefreshToken =
        new(
            "Authentication.InvalidRefreshToken",
            "The refresh token is invalid or expired.",
            ErrorType.Unauthorized);

    public static readonly Error Unauthorized =
        new(
            "Authentication.Unauthorized",
            "You are not authenticated.",
            ErrorType.Unauthorized);

    public static readonly Error Forbidden =
        new(
            "Authentication.Forbidden",
            "You are not authorized to perform this action.",
            ErrorType.Forbidden);

    public static readonly Error InvalidPasswordResetToken =
        new(
            "Authentication.InvalidPasswordResetToken",
            "The password reset link is invalid or has expired.",
            ErrorType.Validation);

    public static readonly Error InvalidEmailConfirmationToken =
        new(
            "Authentication.InvalidEmailConfirmationToken",
            "The email confirmation link is invalid or has expired.",
            ErrorType.Validation);

    public static readonly Error AlreadyConfirmed =
        new(
            "Authentication.AlreadyConfirmed",
            "The email Is Already Confirmed.",
            ErrorType.Conflict);
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.UpdateProfileImage;

public sealed class UpdateProfileImageCommandValidator
    : AbstractValidator<UpdateProfileImageCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private const long MaxFileSize = 5 * 1024 * 1024;

    public UpdateProfileImageCommandValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Profile image is required.");

        RuleFor(x => x.Image)
            .Must(x => x is not null && x.Length > 0)
            .WithMessage("Profile image cannot be empty.");

        RuleFor(x => x.Image)
            .Must(x =>
                x is not null &&
                AllowedContentTypes.Contains(x.ContentType))
            .WithMessage("Only JPG, PNG, and WEBP images are allowed.");

        RuleFor(x => x.Image)
            .Must(x =>
                x is not null &&
                x.Length <= MaxFileSize)
            .WithMessage("Profile image must not exceed 5 MB.");
    }
}
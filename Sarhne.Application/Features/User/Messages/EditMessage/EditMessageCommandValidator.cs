using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.EditMessage;

public sealed class EditMessageCommandValidator
    : AbstractValidator<EditMessageCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private const long MaxFileSize = 5 * 1024 * 1024;

    public EditMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .GreaterThan(0);

        RuleFor(x => x.Content)
            .MaximumLength(5000)
            .When(x => x.Content is not null);

        RuleFor(x => x.Photo)
            .Must(x =>
                x is null ||
                AllowedContentTypes.Contains(x.ContentType))
            .WithMessage(
                "Only JPG, PNG, and WEBP images are allowed.");

        RuleFor(x => x.Photo)
            .Must(x =>
                x is null ||
                x.Length <= MaxFileSize)
            .WithMessage(
                "Message photo must not exceed 5 MB.");

        RuleFor(x => x)
            .Must(x =>
                !x.RemoveContent ||
                x.Content is null)
            .WithMessage(
                "Content cannot be provided when RemoveContent is true.");

        RuleFor(x => x)
            .Must(x =>
                !x.RemovePhoto ||
                x.Photo is null)
            .WithMessage(
                "Photo cannot be provided when RemovePhoto is true.");

        RuleFor(x => x)
            .Must(x =>
                x.Content is not null ||
                x.Photo is not null ||
                x.RemoveContent ||
                x.RemovePhoto)
            .WithMessage(
                "There is nothing to update.");
    }
}
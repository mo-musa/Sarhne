using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.SendMessage;

public sealed class SendMessageCommandValidator
    : AbstractValidator<SendMessageCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private const long MaxFileSize = 5 * 1024 * 1024;
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Content)
            .MaximumLength(5000)
            .When(x => x.Content is not null);

        RuleFor(x => x.Photo)
            .Must(x =>
                x is null ||
                AllowedContentTypes.Contains(
                    x.ContentType.ToLowerInvariant()))
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
                !string.IsNullOrWhiteSpace(x.Content) || x.Photo != null)
            .WithMessage(
                "Message must contain content or a photo.");
    }
}
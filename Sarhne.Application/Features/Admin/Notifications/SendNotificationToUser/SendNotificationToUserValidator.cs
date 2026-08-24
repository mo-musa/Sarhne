using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Notifications.SendNotificationToUser;

public sealed class SendNotificationToUserCommandValidator
    : AbstractValidator<SendNotificationToUserCommand>
{
    public SendNotificationToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Body)
            .NotEmpty()
            .MaximumLength(5000);
    }
}
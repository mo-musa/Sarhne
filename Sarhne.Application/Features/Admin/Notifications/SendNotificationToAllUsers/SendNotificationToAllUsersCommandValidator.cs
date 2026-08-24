using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Notifications.SendNotificationToAllUsers;

public sealed class SendNotificationToAllUsersCommandValidator
    : AbstractValidator<SendNotificationToAllUsersCommand>
{
    public SendNotificationToAllUsersCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Body)
            .NotEmpty()
            .MaximumLength(5000);
    }
}
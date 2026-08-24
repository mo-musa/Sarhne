using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.UserSettings.UpdateUserSettings;

public sealed class UpdateUserSettingsCommandValidator
    : AbstractValidator<UpdateUserSettingsCommand>
{
    public UpdateUserSettingsCommandValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                x.AllowAnonymousMessages.HasValue ||
                x.AllowMessages.HasValue ||
                x.AllowSendPhoto.HasValue ||
                x.AllowReceiveEmail.HasValue ||
                x.AllowReceiveNotifications.HasValue ||
                x.ShowLastSeen.HasValue ||
                x.ShowProfileViewsCount.HasValue)
            .WithMessage("At least one setting must be provided.");
    }
}
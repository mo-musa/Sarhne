using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.UserSettings.GetUserSettings;

public sealed record GetUserSettingsQuery
    : IRequest<Result<UserSettingsResponseDto>>;
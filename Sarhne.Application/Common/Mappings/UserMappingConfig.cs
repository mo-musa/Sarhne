using Mapster;
using Sarhne.Application.Features.User.UserSettings.GetUserSettings;
using Sarhne.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Mappings;

public sealed class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserSetting, UserSettingsResponseDto>();
    }
}
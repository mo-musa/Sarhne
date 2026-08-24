using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Caching;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.UserSettings.GetUserSettings;

public sealed class GetUserSettingsQueryHandler
    : IRequestHandler<
        GetUserSettingsQuery,
        Result<UserSettingsResponseDto>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetUserSettingsQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ICacheService cacheService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _cacheService = cacheService;
    }
    //Todo => use caching here
    public async Task<Result<UserSettingsResponseDto>> Handle(
        GetUserSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId is null)
        {
            return UserErrors.UserNotFound;
        }

        var cacheKey = CacheKeys.UserSettings(userId.Value);

        var cachedSettings =
            await _cacheService.GetAsync<UserSettingsResponseDto>(
                cacheKey);

        if (cachedSettings is not null)
        {
            return cachedSettings;
        }

        var settings = await _context.UserSettings
            .AsNoTracking()
            //.Where(x => x.UserId == userId)
            //.Select(x => new UserSettingsResponseDto
            //{
            //    AllowAnonymousMessages = x.AllowAnonymousMessages,
            //    AllowMessages = x.AllowMessages,
            //    AllowSendPhoto = x.AllowSendPhoto,
            //    AllowReceiveEmail = x.AllowReceiveEmail,
            //    AllowReceiveNotifications = x.AllowReceiveNotifications,
            //    ShowLastSeen = x.ShowLastSeen,
            //    ShowProfileViewsCount = x.ShowProfileViewsCount
            //})
            .FirstOrDefaultAsync(x => x.UserId == userId,cancellationToken);

        if (settings is null)
        {
            return UserErrors.UserNotFound;
        }

        var response = _mapper.Map<UserSettingsResponseDto>(settings);

        await _cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(15));

        return response;
    }
}
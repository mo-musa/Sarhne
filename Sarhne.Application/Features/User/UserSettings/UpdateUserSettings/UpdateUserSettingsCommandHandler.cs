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

namespace Sarhne.Application.Features.User.UserSettings.UpdateUserSettings;

public sealed class UpdateUserSettingsCommandHandler
    : IRequestHandler<UpdateUserSettingsCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;

    public UpdateUserSettingsCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        ICacheService cacheService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(
        UpdateUserSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId is null)
        {
            return UserErrors.UserNotFound;
        }

        var settings = await _context.UserSettings
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (settings is null)
        {
            return UserErrors.UserNotFound;
        }

        if (request.AllowAnonymousMessages.HasValue)
            settings.AllowingAnonymousMessages(request.AllowAnonymousMessages.Value);

        if (request.AllowMessages.HasValue)
            settings.AllowingMessages(request.AllowMessages.Value);

        if (request.AllowSendPhoto.HasValue)
            settings.AllowingSendPhoto(request.AllowSendPhoto.Value);

        if (request.AllowReceiveEmail.HasValue)
            settings.AllowingReceiveEmail(request.AllowReceiveEmail.Value);

        if (request.AllowReceiveNotifications.HasValue)
            settings.AllowingReceiveNotifications(request.AllowReceiveNotifications.Value);

        if (request.ShowLastSeen.HasValue)
            settings.ShowingLastSeen(request.ShowLastSeen.Value);

        if (request.ShowProfileViewsCount.HasValue)
            settings.ShowingProfileViewsCount(request.ShowProfileViewsCount.Value);

        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(
            CacheKeys.UserSettings(userId.Value));

        return Result.Success();
    }
}

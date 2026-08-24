using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Common.Dtos;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Notifications;
using Sarhne.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.FollowUser;

public sealed class FollowUserCommandHandler
    : IRequestHandler<FollowUserCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly INotificationRealtimeService _notificationRealtimeService;
    public FollowUserCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        INotificationRealtimeService notificationRealtimeService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _notificationRealtimeService = notificationRealtimeService;
    }

    public async Task<Result> Handle(
        FollowUserCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return UserErrors.UserNotFound;
        }

        var targetUser = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserName == request.UserName,
                cancellationToken);

        if (targetUser is null)
        {
            return UserErrors.UserNotFound;
        }

        if (targetUser.Id == currentUserId)
        {
            return UserErrors.CannotFollowYourself;
        }

        var alreadyFollowing = await _context.Followings
            .AnyAsync(
                x =>
                    x.FollowerId == currentUserId &&
                    x.CreatorId == targetUser.Id,
                cancellationToken);

        if (alreadyFollowing)
        {
            return UserErrors.AlreadyFollowing;
        }

        var following = new Following(currentUserId.Value, targetUser.Id);

        _context.Followings.Add(following);

        var notification = await _notificationService.CreateFollowNotificationAsync(
            currentUserId.Value,
            targetUser.Id,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        if (notification != null)
        {
            var notificationRealTime = await _context.Notifications
            .AsNoTracking()
            .Where(x => x.Id == notification!.Id)
            .Select(x => new
            {
                Dto = new NotificationRealtimeDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    Title = x.Title,
                    Body = x.Body,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt,
                    SenderId = x.SenderId,
                    SenderUserName = x.Sender != null
                        ? x.Sender.UserName
                        : null,
                    SenderImageUrl = x.Sender != null
                        ? x.Sender.ImageUrl
                        : null
                },
                x.ReceiverId
            })
            .FirstOrDefaultAsync(cancellationToken);

            if (notificationRealTime is not null)
            {
                await _notificationRealtimeService.SendNotificationAsync(
                    notificationRealTime!.ReceiverId,
                    notificationRealTime.Dto,
                    cancellationToken);
            }
        }
        return Result.Success();
    }
}
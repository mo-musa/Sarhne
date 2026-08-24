using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.UnfollowUser;

public sealed class UnfollowUserCommandHandler
    : IRequestHandler<UnfollowUserCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UnfollowUserCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        UnfollowUserCommand request,
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

        var following = await _context.Followings
            .FirstOrDefaultAsync(
                x =>
                    x.FollowerId == currentUserId.Value &&
                    x.CreatorId == targetUser.Id,
                cancellationToken);

        if (following is null)
        {
            return UserErrors.NotFollowing;
        }

        _context.Followings.Remove(following);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
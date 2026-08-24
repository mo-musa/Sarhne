using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.GetUserByUsername;

public sealed class GetUserByUsernameQueryHandler
    : IRequestHandler<
        GetUserByUsernameQuery,
        Result<UserResponseDto>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserByUsernameQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserResponseDto>> Handle(
        GetUserByUsernameQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var user = await _context.Users
            .AsNoTracking()
            .Where(x => x.UserName == request.UserName)
            .Select(x => new
            {
                x.Id,

                Response = new UserResponseDto
                {
                    FullName = x.FullName,
                    AboutMe = x.AboutMe,
                    ImageUrl = x.ImageUrl,
                    Gender = x.Gender,

                    FollowingCount = x.Following.Count(),
                    FollowersCount = x.Followers.Count(),

                    VisitorsCount = x.UserSetting.ShowProfileViewsCount
                        ? x.VisitorsCount
                        : null,

                    LastSeen = x.UserSetting.ShowLastSeen
                        ? x.LastSeen
                        : null,
                    IsFollowing = currentUserId.HasValue
                        && x.Followers.Any(f =>
                            f.FollowerId == currentUserId.Value)
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        var isOwner =
            _currentUserService.IsAuthenticated &&
            _currentUserService.UserId == user.Id;

        if (!isOwner)
        {
            await _context.Users
                .Where(x => x.Id == user.Id)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(
                        x => x.VisitorsCount,
                        x => x.VisitorsCount + 1),
                    cancellationToken);
        }

        return user.Response;
    }
}
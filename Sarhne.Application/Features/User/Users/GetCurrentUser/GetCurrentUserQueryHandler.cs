using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Features.User.Users.GetUserByUsername;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<
        GetCurrentUserQuery,
        Result<CurrentUserResponseDto>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CurrentUserResponseDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId is null)
        {
            return UserErrors.UserNotFound;
        }

        var user = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new CurrentUserResponseDto
            {
                FullName = x.FullName,
                AboutMe = x.AboutMe,
                ImageUrl = x.ImageUrl,
                Gender = x.Gender,

                FollowingCount = x.Following.Count(),
                FollowersCount = x.Followers.Count(),

                VisitorsCount = x.VisitorsCount,

                LastSeen = x.LastSeen
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        return user;
    }
}
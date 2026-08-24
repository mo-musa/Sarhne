using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.GetFollowing;

public sealed class GetFollowingQueryHandler
    : IRequestHandler<
        GetFollowingQuery,
        Result<PagedResult<FollowingUserResponseDto>>>
{
    private readonly ISarhneDbContext _context;

    public GetFollowingQueryHandler(
        ISarhneDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<FollowingUserResponseDto>>> Handle(
        GetFollowingQuery request,
        CancellationToken cancellationToken)
    {
        var userId = await _context.Users
            .AsNoTracking()
            .Where(x => x.UserName == request.UserName)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId is null)
        {
            return UserErrors.UserNotFound;
        }

        var query = _context.Followings
            .AsNoTracking()
            .Where(x => x.FollowerId == userId.Value);

        var totalCount = await query
            .CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
            .Select(x => new FollowingUserResponseDto
            {
                UserName = x.Creator.UserName!,
                FullName = x.Creator.FullName!,
                ImageUrl = x.Creator.ImageUrl
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<FollowingUserResponseDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
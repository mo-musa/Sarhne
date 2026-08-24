using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Followings.GetFollowers;

public sealed class GetFollowersQueryHandler
    : IRequestHandler<
        GetFollowersQuery,
        Result<PagedResult<FollowerResponseDto>>>
{
    private readonly ISarhneDbContext _context;

    public GetFollowersQueryHandler(
        ISarhneDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<FollowerResponseDto>>> Handle(
        GetFollowersQuery request,
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
            .Where(x => x.CreatorId == userId.Value);

        var totalCount = await query
            .CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
            .Select(x => new FollowerResponseDto
            {
                UserName = x.Follower.UserName!,
                FullName = x.Follower.FullName!,
                ImageUrl = x.Follower.ImageUrl
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<FollowerResponseDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
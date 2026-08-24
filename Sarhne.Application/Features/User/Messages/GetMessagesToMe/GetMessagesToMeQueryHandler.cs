using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Features.User.Messages.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetMessagesToMe;

public sealed class GetMessagesToMeQueryHandler
    : IRequestHandler<
        GetMessagesToMeQuery,
        Result<PagedResult<MessageResponse>>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMessagesToMeQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<MessageResponse>>> Handle(
        GetMessagesToMeQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var query = _context.Messages
            .AsNoTracking()
            .Where(x => x.ReceiverId == currentUserId.Value);

        if (request.IsRead.HasValue)
        {
            query = query.Where(
                x => x.IsRead == request.IsRead.Value);
        }

        if (request.IsStarred.HasValue)
        {
            query = query.Where(
                x => x.IsStarred == request.IsStarred.Value);
        }

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new MessageResponse(
                x.Id,
                x.Content,
                x.PhotoUrl,
                x.IsAnonymous,
                x.IsRead,
                x.IsStarred,
                x.LikesCount,
                x.CreatedAt,

                x.IsAnonymous
                    ? null
                    : x.SenderId,

                x.IsAnonymous
                    ? null
                    : x.Sender != null
                        ? x.Sender.UserName
                        : null,

                x.IsAnonymous
                    ? null
                    : x.Sender != null
                        ? x.Sender.ImageUrl
                        : null
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<MessageResponse>
        {
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Items = items
        };

        return result;
    }
}

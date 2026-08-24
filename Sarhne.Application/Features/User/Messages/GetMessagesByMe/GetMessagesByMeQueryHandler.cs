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

namespace Sarhne.Application.Features.User.Messages.GetMessagesByMe;

public sealed class GetMessagesByMeQueryHandler
    : IRequestHandler<
        GetMessagesByMeQuery,
        Result<PagedResult<MessageResponse>>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMessagesByMeQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<MessageResponse>>> Handle(
        GetMessagesByMeQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var query = _context.Messages
            .AsNoTracking()
            .Where(x => x.SenderId == currentUserId.Value);

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
                x.SenderId,

                x.Sender != null
                    ? x.Sender.UserName
                    : null,

                x.Sender != null
                    ? x.Sender.ImageUrl
                    : null
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<MessageResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return result;
    }
}
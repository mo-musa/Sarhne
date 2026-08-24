using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Notifications.GetNotifications;

public sealed class GetNotificationsQueryHandler
    : IRequestHandler<
        GetNotificationsQuery,
        Result<PagedResult<NotificationResponseDto>>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetNotificationsQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<NotificationResponseDto>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var query = _context.Notifications
            .AsNoTracking()
            .Where(x => x.ReceiverId == currentUserId);

        var totalCount = await query.CountAsync(cancellationToken);

        var notifications = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new NotificationResponseDto
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
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<NotificationResponseDto>
        {
            Items = notifications,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}

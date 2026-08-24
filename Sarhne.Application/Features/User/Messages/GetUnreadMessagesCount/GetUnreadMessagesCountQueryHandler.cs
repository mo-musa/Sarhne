using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetUnreadMessagesCount;

public sealed class GetUnreadMessagesCountQueryHandler
    : IRequestHandler<
        GetUnreadMessagesCountQuery,
        Result<int>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUnreadMessagesCountQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(
        GetUnreadMessagesCountQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var count = await _context.Messages
            .AsNoTracking()
            .CountAsync(
                x =>
                    x.ReceiverId == currentUserId.Value &&
                    !x.IsRead,
                cancellationToken);

        return count;
    }
}
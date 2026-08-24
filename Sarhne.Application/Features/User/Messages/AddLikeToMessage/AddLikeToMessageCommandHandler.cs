using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Caching;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.AddLikeToMessage;

public sealed class AddLikeToMessageCommandHandler
    : IRequestHandler<AddLikeToMessageCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;

    public AddLikeToMessageCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        ICacheService cacheService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(
        AddLikeToMessageCommand request,
        CancellationToken cancellationToken)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.MessageId &&
                    !x.IsHidden,
                cancellationToken);

        if (message is null)
            return MessageErrors.MessageNotFound;

        var currentUserId = _currentUserService.UserId;

        if (currentUserId is not null &&
            message.ReceiverId == currentUserId.Value)
        {
            return MessageErrors.CannotLikeOwnMessage;
        }

        message.AddLike();

        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
            CacheKeys.PublicMessagesPrefix(message.ReceiverId));

        return Result.Success();
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Caching;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.GetPublicMessagesByUser;

public sealed class GetPublicMessagesByUserQueryHandler
    : IRequestHandler<
        GetPublicMessagesByUserQuery,
        Result<PagedResult<PublicMessageResponse>>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICacheService _cacheService;

    public GetPublicMessagesByUserQueryHandler(
        ISarhneDbContext context,
        ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<PagedResult<PublicMessageResponse>>> Handle(
        GetPublicMessagesByUserQuery request,
        CancellationToken cancellationToken)
    {

        var cacheKey = CacheKeys.PublicMessages(
            request.UserId,
            request.PageNumber,
            request.PageSize);

        var cachedResult =
            await _cacheService
                .GetAsync<PagedResult<PublicMessageResponse>>(
                    cacheKey);

        if (cachedResult is not null)
            return cachedResult;

        var query = _context.Messages
            .AsNoTracking()
            .Where(x =>
                x.ReceiverId == request.UserId &&
                !x.IsHidden);

        var totalCount = await query
            .CountAsync(cancellationToken);

        var messages = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
            .Select(x => new PublicMessageResponse
            {
                Id = x.Id,

                Content = x.Content,

                PhotoUrl = x.PhotoUrl,

                IsAnonymous = x.IsAnonymous,

                LikesCount = x.LikesCount,

                CreatedAt = x.CreatedAt,

                SenderId = x.IsAnonymous
                    ? null
                    : x.SenderId,

                SenderUserName = x.IsAnonymous
                    ? null
                    : x.Sender != null
                        ? x.Sender.UserName
                        : null,

                SenderImageUrl = x.IsAnonymous
                    ? null
                    : x.Sender != null
                        ? x.Sender.ImageUrl
                        : null
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<PublicMessageResponse>
        {
            Items = messages,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        await _cacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(5));

        return result;
    }
}
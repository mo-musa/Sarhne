using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Caching;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Caching;
using Sarhne.Application.Contracts.Services.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.EditMessage;

public sealed class EditMessageCommandHandler
    : IRequestHandler<EditMessageCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICacheService _cacheService;

    public EditMessageCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        ICacheService cacheService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(
     EditMessageCommand request,
     CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var message = await _context.Messages
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.MessageId &&
                    x.SenderId == currentUserId.Value,
                cancellationToken);

        if (message is null)
            return MessageErrors.MessageNotFound;

        if (request.RemoveContent)
        {
            message.RemoveContent();
        }
        else if (request.Content is not null)
        {
            message.EditContent(request.Content);
        }

        string? oldPhotoUrl = null;

        if (request.RemovePhoto)
        {
            oldPhotoUrl = message.PhotoUrl;

            message.RemovePhoto();
        }
        else if (request.Photo is not null)
        {
            oldPhotoUrl = message.PhotoUrl;

            await using var stream =
                request.Photo.OpenReadStream();

            var photoUrl =
                await _fileStorageService.UploadAsync(
                    stream,
                    request.Photo.FileName,
                    request.Photo.ContentType,
                    "messages",
                    cancellationToken);

            message.EditPhoto(photoUrl);
        }

        if (string.IsNullOrWhiteSpace(message.Content) &&
            string.IsNullOrWhiteSpace(message.PhotoUrl))
        {
            return MessageErrors.MessageContentOrPhotoRequired;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
            CacheKeys.PublicMessagesPrefix(message.ReceiverId));

        if (oldPhotoUrl is not null)
        {
            await _fileStorageService.DeleteAsync(
                oldPhotoUrl,
                cancellationToken);
        }

        return Result.Success();
    }
}
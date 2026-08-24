using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Caching;
using Sarhne.Application.Common.Dtos;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Caching;
using Sarhne.Application.Contracts.Services.Messages;
using Sarhne.Application.Contracts.Services.Notifications;
using Sarhne.Application.Contracts.Services.Storage;
using Sarhne.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.SendMessage;

public sealed class SendMessageCommandHandler
    : IRequestHandler<SendMessageCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly IMessageRealtimeService _messageRealtimeService;
    private readonly INotificationRealtimeService _notificationRealtimeService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICacheService _cacheService;

    public SendMessageCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        IMessageRealtimeService messageRealtimeService,
        INotificationRealtimeService notificationRealtimeService,
        IFileStorageService fileStorageService,
        ICacheService cacheService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _messageRealtimeService = messageRealtimeService;
        _notificationRealtimeService = notificationRealtimeService;
        _fileStorageService = fileStorageService;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
            return UserErrors.UserNotFound;

        var receiver = await _context.Users
            .FirstOrDefaultAsync(
                x => x.UserName == request.UserName,
                cancellationToken);

        if (receiver is null)
            return MessageErrors.ReceiverNotFound;

        if (receiver.Id == currentUserId.Value)
            return MessageErrors.CannotMessageYourself;

        //=========================================================
        var cacheKey = CacheKeys.UserSettings(receiver.Id);

        var receiverSettings =
            await _cacheService.GetAsync<UserSetting>(cacheKey);

        if (receiverSettings is null)
        {
            receiverSettings = await _context.UserSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.UserId == receiver.Id,
                    cancellationToken);

            if (receiverSettings is null)
                return UserErrors.UserNotFound;

            await _cacheService.SetAsync(
                cacheKey,
                receiverSettings,
                TimeSpan.FromMinutes(15));
        }
        //=========================================================

        if (receiverSettings is null)
            return UserErrors.UserNotFound;

        if (!receiverSettings.AllowMessages)
            return MessageErrors.MessagesNotAllowed;

        if (request.IsAnonymous && !receiverSettings.AllowAnonymousMessages)
            return MessageErrors.AnonymousMessagesNotAllowed;

        if (request.Photo is not null && !receiverSettings.AllowSendPhoto)
            return MessageErrors.PhotoMessagesNotAllowed;

        string? photoUrl = null;

        if (request.Photo is not null)
        {
            await using var photoStream = request.Photo.OpenReadStream();

            photoUrl = await _fileStorageService.UploadAsync(
                photoStream,
                request.Photo.FileName,
                request.Photo.ContentType,
                "messages",
                cancellationToken);
        }

        var message = new Message(
            currentUserId.Value,
            receiver.Id,
            request.Content,
            photoUrl,
            request.IsAnonymous);

        _context.Messages.Add(message);

        var notification =
        await _notificationService.CreateMessageNotificationAsync(
            currentUserId.Value,
            receiver.Id,
            cancellationToken);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            if (photoUrl is not null)
            {
                await _fileStorageService.DeleteAsync(
                    photoUrl,
                    cancellationToken);
            }

            throw;
        }
        if (notification != null)
        {
            var notificationRealTime = await _context.Notifications
            .AsNoTracking()
            .Where(x => x.Id == notification!.Id)
            .Select(x => new
            {
                Dto = new NotificationRealtimeDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    Title = x.Title,
                    Body = x.Body,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt,
                    SenderId = request.IsAnonymous ? null : x.SenderId,

                    SenderUserName = request.IsAnonymous ? null
                    : x.Sender != null ? x.Sender.UserName
                        : null,

                    SenderImageUrl = request.IsAnonymous ? null
                    : x.Sender != null ? x.Sender.ImageUrl
                        : null
                },
                x.ReceiverId
            })
            .FirstOrDefaultAsync(cancellationToken);

            if (notificationRealTime is not null)
            {
                await _notificationRealtimeService.SendNotificationAsync(
                    notificationRealTime!.ReceiverId,
                    notificationRealTime.Dto,
                    cancellationToken);
            }
        }

        var realtimeMessage = await _context.Messages
            .AsNoTracking()
            .Where(x => x.Id == message.Id)
            .Select(x => new MessageRealtimeDto
            {
                Id = x.Id,
                Content = x.Content,
                PhotoUrl = x.PhotoUrl,
                IsAnonymous = x.IsAnonymous,
                IsRead = x.IsRead,
                IsStarred = x.IsStarred,
                LikesCount = x.LikesCount,
                CreatedAt = x.CreatedAt,

                SenderId = x.IsAnonymous
                    ? null
                    : x.SenderId,

                SenderUserName = x.IsAnonymous
                    ? null
                    : x.Sender!.UserName,

                SenderImageUrl = x.IsAnonymous
                    ? null
                    : x.Sender!.ImageUrl
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (realtimeMessage is not null)
        {
            await _messageRealtimeService.SendMessageAsync(
                receiver.Id,
                realtimeMessage,
                cancellationToken);
        }

        return Result.Success();
    }
}
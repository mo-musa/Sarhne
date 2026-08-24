using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.UpdateProfileImage;

public sealed class UpdateProfileImageCommandHandler
    : IRequestHandler<
        UpdateProfileImageCommand,
        Result<string>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public UpdateProfileImageCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<string>> Handle(
        UpdateProfileImageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId is null)
        {
            return UserErrors.UserNotFound;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        var oldImageUrl = user.ImageUrl;

        await using var stream = request.Image.OpenReadStream();

        var imageUrl = await _fileStorageService.UploadAsync(
            stream,
            request.Image.FileName,
            request.Image.ContentType,
            "profile-images",
            cancellationToken);

        user.ImageUrl = imageUrl;

        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldImageUrl))
        {
            await _fileStorageService.DeleteAsync(
                oldImageUrl,
                cancellationToken)  ;
        }

        return imageUrl;
    }
}
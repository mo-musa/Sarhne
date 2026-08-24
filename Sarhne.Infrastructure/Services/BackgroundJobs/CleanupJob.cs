using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.BackgroundJobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Infrastructure.Services.BackgroundJobs;

public sealed class CleanupJob : ICleanupJob
{
    private readonly ISarhneDbContext _context;
    private readonly ILogger<CleanupJob> _logger;

    public CleanupJob(ISarhneDbContext context, ILogger<CleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken = default)
    {

        _logger.LogInformation(
            "Cleanup job started at {Time}",
            DateTime.UtcNow);

        var notificationsCutoff =
            DateTime.UtcNow.AddMonths(-2);

        var expiredNotifications = await _context.Notifications
            .Where(x => x.CreatedAt < notificationsCutoff)
            .ToListAsync(cancellationToken);

        if (expiredNotifications.Count > 0)
        {
            _context.Notifications.RemoveRange(
                expiredNotifications);
        }

        var expiredRefreshTokens = await _context.RefreshTokens
            .Where(x => x.ExpiresOnUtc <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        if (expiredRefreshTokens.Count > 0)
        {
            _context.RefreshTokens.RemoveRange(
                expiredRefreshTokens);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Cleanup job deleted {NotificationsCount} notifications and {RefreshTokensCount} refresh tokens.",
            expiredNotifications.Count,
            expiredRefreshTokens.Count);
    }
}
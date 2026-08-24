using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sarhne.Domain.Entities;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Persistence;

public interface ISarhneDbContext
{
    DbSet<UserSetting> UserSettings { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<ApplicationRole> Roles { get; }
    DbSet<ApplicationUserRole> UserRoles { get; }

    DbSet<Notification> Notifications { get; }

    DbSet<Message> Messages { get; }

    DbSet<Following> Followings { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    DatabaseFacade Database {  get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
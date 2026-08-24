using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Domain.Entities;
using Sarhne.Domain.Entities.Identity;

namespace Sarhne.Infrastructure.Persistence;

public class SarhneDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    int,
    IdentityUserClaim<int>,
    ApplicationUserRole,
    IdentityUserLogin<int>,
    IdentityRoleClaim<int>, 
    IdentityUserToken<int>>, ISarhneDbContext
{
    public SarhneDbContext(DbContextOptions<SarhneDbContext> options) : base(options)
    {
    }
    public DbSet<UserSetting> UserSettings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Following> Followings { get; set; }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(SarhneDbContext).Assembly);

        //builder.Entity<ISoftDeletableEntity>()
        //.HasQueryFilter(x => !x.IsDeleted);

    }

}

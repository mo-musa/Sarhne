using Microsoft.AspNetCore.Identity;
using Sarhne.Domain.Common;
using Sarhne.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sarhne.Domain.Entities.Identity;
public class ApplicationUser : IdentityUser<int>, ISoftDeletableEntity, IAuditableEntity
{
    #region Properties
    public string FullName { get; set; } = null!;

    public Gender Gender { get; set; } = Gender.Male;

    public string AccountLink { get; private set; }

    public string? AboutMe { get; set; }

    public DateTime? LastSeen { get; set; }

    public string? ImageUrl { get; set; }

    public int VisitorsCount { get; set; } = 0;
    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
    public ApplicationUser()
    {

    }
    public ApplicationUser(
        string userName,
        string email)
    {
        UserName = userName;
        Email = email;

        AccountLink = GenerateAccountLink(userName);
    }

    private static string GenerateAccountLink(string userName)
    {
        return $"https://{userName.Trim().ToLower().Replace(" ", "")}.sarhne.com";
    }
    #endregion
    //______________________________________________________________________

    #region Audit & Delete
    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }

    // Soft Delete
    public DateTime? DeletedAt { get; set; }
    public int? DeletedById { get; set; }
    public bool IsDeleted { get; set; }
    public void SoftDelete(int? deletedById)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedById = deletedById;
    }
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedById = null;
    }
    #endregion
    //______________________________________________________________________
    #region Relations
    // Navigation Properties

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new HashSet<ApplicationUserRole>();

    public UserSetting UserSetting { get; set; } = null!;

    public ICollection<Message> ReceivedMessages { get; set; } = new HashSet<Message>();

    public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();

    public ICollection<Notification> ReceivedNotifications { get; set; } = new HashSet<Notification>();
    public ICollection<Notification> SentNotifications { get; set; } = new HashSet<Notification>();

    public ICollection<Following> Following { get; set; } = new HashSet<Following>();

    public ICollection<Following> Followers { get; set; } = new HashSet<Following>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    #endregion
}

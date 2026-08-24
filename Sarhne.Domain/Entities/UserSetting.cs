using Sarhne.Domain.Common;
using Sarhne.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Sarhne.Domain.Entities;

public class UserSetting : BaseEntity
{

    public bool AllowAnonymousMessages { get; private set; } = true;

    public bool AllowMessages { get; private set; } = true;

    public bool AllowSendPhoto { get; private set; } = true;

    public bool AllowReceiveEmail { get; private set; } = true;

    public bool AllowReceiveNotifications { get; private set; } = true;

    public bool ShowLastSeen { get; private set; } = true;

    public bool ShowProfileViewsCount { get; private set; } = true;
    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public void AllowingAnonymousMessages(bool allow)
    {
        AllowAnonymousMessages = allow;
    }
    public void AllowingMessages(bool allow)
    {
        AllowMessages = allow;
    }
    public void AllowingSendPhoto(bool allow)
    {
        AllowSendPhoto = allow;
    }
    public void AllowingReceiveEmail(bool allow)
    {
        AllowReceiveEmail = allow;
    }
    public void AllowingReceiveNotifications(bool allow)
    {
        AllowReceiveNotifications = allow;
    }
    public void ShowingLastSeen(bool allow)
    {
        ShowLastSeen = allow;
    }
    public void ShowingProfileViewsCount(bool allow)
    {
        ShowProfileViewsCount = allow;
    }
}

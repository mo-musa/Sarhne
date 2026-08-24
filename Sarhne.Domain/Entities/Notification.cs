using Sarhne.Domain.Common;
using Sarhne.Domain.Entities.Identity;
using Sarhne.Domain.Enums;

namespace Sarhne.Domain.Entities;

public class Notification : BaseEntity
{
    public NotificationType Type { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = string.Empty;

    public bool IsRead { get; private set; } = false;
    //_________________________________________________________
    public int ReceiverId { get; set; }

    public ApplicationUser Receiver { get; set; } = null!;

    public int? SenderId { get; set; }

    public ApplicationUser? Sender { get; set; } = null;
    //_________________________________________________________
    private Notification()
    {
        
    }
    public Notification(NotificationType type, string title, string body, int? senderId, int receiverId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");
        Type = type;
        Title = title;
        Body = body;
        SenderId = senderId;
        ReceiverId = receiverId;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}

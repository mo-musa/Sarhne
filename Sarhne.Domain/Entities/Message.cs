using Sarhne.Domain.Common;
using Sarhne.Domain.Entities.Identity;

namespace Sarhne.Domain.Entities;

public class Message : BaseEntity
{
    public string? Content { get; private set; }

    public string? PhotoUrl { get; private set; }

    public bool IsRead { get; private set; } = false;

    public bool IsStarred { get; private set; } = false;

    public int LikesCount { get; private set; } = 0;

    public bool IsHidden { get; private set; } = true;
    public bool IsAnonymous { get; private set; } = true;

    public int ReceiverId { get; set; }

    public ApplicationUser Receiver { get; set; } = null!;

    public int SenderId { get; set; }

    public ApplicationUser Sender { get; set; } = null!;

    //_________________________________________________________________
    private Message()
    {
    }
    public Message(int senderId, int receiverId, string? content, string? photoUrl, bool isAnonymous)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(photoUrl))
            throw new ArgumentException("Message must contain content or a photo.");

        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        PhotoUrl = photoUrl;
        IsRead = false;
        IsStarred = false;
        IsAnonymous = isAnonymous;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }

    public void ToggleStar()
    {
        IsStarred = !IsStarred;
    }
    public void AddLike()
    {
        LikesCount++;
    }
    public void SetHidden(bool hidden)
    {
        IsHidden = hidden;
    }
    public void EditContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty.");
        Content = content;
    }
    public void EditPhoto(string photourl)
    {
        if (string.IsNullOrWhiteSpace(photourl))
            throw new ArgumentException("Photo cannot be empty.");
        PhotoUrl = photourl;
    }
    public void RemoveContent()
    {
        Content = null;
    }

    public void RemovePhoto()
    {
        PhotoUrl = null;
    }
}

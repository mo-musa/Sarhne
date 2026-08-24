using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sarhne.Domain.Entities.Identity;

public class RefreshToken
{
    public long Id { get; set; }

    [MaxLength(500)]
    public string Token { get; set; } = default!;

    public DateTime ExpiresOnUtc { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime? RevokedOnUtc { get; set; }

    [MaxLength(100)]
    public string? CreatedByIp { get; set; }

    [MaxLength(100)]
    public string? RevokedByIp { get; set; }

    [MaxLength(500)]
    public string? ReplacedByToken { get; set; }

    [MaxLength(500)]
    public string? RevocationReason { get; set; }

    public int UserId { get; set; }

    public ApplicationUser User { get; set; } = default!;

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresOnUtc;

    public bool IsRevoked =>
        RevokedOnUtc is not null;

    public bool IsActive =>
        !IsExpired && !IsRevoked;

    public void Revoke(string? replacedByToken, string? ipAddress, string? reason)
    {
        RevokedOnUtc = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
        RevokedByIp = ipAddress;
        RevocationReason = reason;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Domain.Common;

public abstract class AuditableEntity : IAuditableEntity
{
    public int Id { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }
}
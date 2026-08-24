using System.ComponentModel.DataAnnotations;

namespace Sarhne.Domain.Common;

public abstract class BaseEntity : IBaseEntity
{
    public int Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedById { get; set; }
    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }
    public bool IsDeleted { get; set; } = false;

}
public interface IBaseEntity : IAuditableEntity, ISoftDeletableEntity
{
    public int Id { get; set; }
}

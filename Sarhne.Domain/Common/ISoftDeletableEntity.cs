namespace Sarhne.Domain.Common;

public interface ISoftDeletableEntity
{
    DateTime? DeletedAt { get;  set; }
    int? DeletedById { get; set; }
    bool IsDeleted { get;  set; }

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
}

namespace RetailPortal.Model.Db.Entities.Common.Base;

public class EntityBase
{
    public ulong Id { get; private set; }
    public Guid Guid { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected EntityBase()
    {
        this.CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdatedDate()
    {
        this.UpdatedAt = DateTime.UtcNow;
    }
}
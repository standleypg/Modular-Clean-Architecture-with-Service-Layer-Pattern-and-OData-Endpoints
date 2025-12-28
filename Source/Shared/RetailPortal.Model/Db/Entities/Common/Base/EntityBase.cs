namespace RetailPortal.Model.Db.Entities.Common.Base;

public class EntityBase
{
    public long Id { get; private set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
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
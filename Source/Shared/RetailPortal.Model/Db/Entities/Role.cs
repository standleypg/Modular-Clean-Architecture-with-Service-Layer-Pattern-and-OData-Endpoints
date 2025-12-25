using RetailPortal.Model.Db.Entities.Common.Base;

namespace RetailPortal.Model.Db.Entities;

public sealed class Role : EntityBase
{
    public string Name { get; private set;}
    public string Description { get; private set;}
    public ICollection<User> Users { get; private set;} = new List<User>();

    private Role(string name, string description)
    {
        this.Name = name;
        this.Description = description;
    }

    public static Role Create(string name, string description)
    {
        return new Role(name, description);
    }
}
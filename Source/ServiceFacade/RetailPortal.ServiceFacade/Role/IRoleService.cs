using RetailPortal.Model.Constants;

namespace RetailPortal.ServiceFacade.Role;

public interface IRoleService
{
    Task<List<Model.Db.Entities.Role>> GetAllRolesAsync();
    Task<Model.Db.Entities.Role> GetRoleByNameAsync(Roles name);
}
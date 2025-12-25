using Microsoft.EntityFrameworkCore;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Constants;
using RetailPortal.ServiceFacade;
using RetailPortal.ServiceFacade.Role;

namespace RetailPortal.Service.Services.Role;

public class RoleService(IReadStore readStore) : IRoleService
{
    public async Task<List<Model.Db.Entities.Role>> GetAllRolesAsync()
    {
        return await readStore.Role.GetAll().ToListAsync();
    }

    public async Task<Model.Db.Entities.Role> GetRoleByNameAsync(Roles name)
    {
        var roles = readStore.Role.GetAll();
        var result = await roles.FirstAsync(x => x.Name == name.ToString());

        return result;
    }
}
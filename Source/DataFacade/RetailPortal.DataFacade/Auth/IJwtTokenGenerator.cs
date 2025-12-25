using RetailPortal.Model.Db.Entities;

namespace RetailPortal.DataFacade.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
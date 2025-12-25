using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Model.DTOs.Auth;

public record AuthResult(
    User User,
    string Token
);
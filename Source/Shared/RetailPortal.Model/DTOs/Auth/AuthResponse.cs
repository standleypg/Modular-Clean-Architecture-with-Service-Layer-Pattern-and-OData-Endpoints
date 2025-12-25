namespace RetailPortal.Model.DTOs.Auth;

public record AuthResponse(
    ulong Id,
    string FirstName,
    string LastName,
    string Email,
    string Token
);
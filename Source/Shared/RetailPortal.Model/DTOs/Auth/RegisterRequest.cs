namespace RetailPortal.Model.DTOs.Auth;

public record RegisterRequest
(
    string FirstName,
    string LastName,
    string Email,
    string Password
);
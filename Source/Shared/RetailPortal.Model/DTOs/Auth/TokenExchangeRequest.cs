namespace RetailPortal.Model.DTOs.Auth;

public record TokenExchangeRequest(string Email, string Name, string TokenProvider);
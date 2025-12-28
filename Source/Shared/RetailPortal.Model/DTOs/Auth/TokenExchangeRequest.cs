using System.Security.Claims;

namespace RetailPortal.Model.DTOs.Auth;

public record TokenExchangeRequest(string Email, string Name, string TokenProvider);

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public string GetClaimValue(string claimType1, string claimType2)
        {
            var claim = principal.FindFirst(claimType1);
            if (claim != null) return claim.Value;

            claim = principal.FindFirst(claimType2);
            return claim != null ? claim.Value : string.Empty;
        }

        public string GetClaimValue(string claimType)
        {
            var claim = principal.FindFirst(claimType);
            return claim != null ? claim.Value : string.Empty;
        }
    }
}
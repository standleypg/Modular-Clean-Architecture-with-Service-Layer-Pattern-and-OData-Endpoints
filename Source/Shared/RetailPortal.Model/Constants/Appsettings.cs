namespace RetailPortal.Model.Constants;

public sealed class Appsettings
{
    public const string PostgresSQLConnectionName = "RetailPortalDb";
    public const string ApiProjectName = "retailportal-api";
    public const string MigrationServiceProjectName = "retailportal-migration-service";

    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string Secret { get; set; } = null!;
        public int ExpirationMinutes { get; set; }
    }

    public class GoogleSettings
    {
        public const string SectionName = "Google";
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string Authority { get; set; } = null!;
    }

    public class AzureAdSettings
    {
        public const string SectionName = "AzureAd";
        public const string JwtBearerScheme = "Azure";
        public string Instance { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string Scopes { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }

}
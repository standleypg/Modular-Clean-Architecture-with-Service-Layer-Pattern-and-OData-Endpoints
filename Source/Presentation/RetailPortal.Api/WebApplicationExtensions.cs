using Microsoft.EntityFrameworkCore;
using RetailPortal.Data.Db.Context;

namespace RetailPortal.Api;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task AddWebApplication()
        {
            await app.AddDevelopment();
        }

        private async Task AddDevelopment()
        {
            if (!app.Environment.IsDevelopment())
            {
                return;
            }

            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v0.json", "API v0.0");
            });

            // Database migrations only executed in development
            // For production, use database migrations tool like EF Core CLI (dotnet ef migrations bundle) and run it in CI/CD pipeline
            // https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli
            // The pipeline should handle the database migrations and update the database schema, this way we can ensure that the deployed application is always up-to-date with the latest migrations
            // otherwise, if the migrations fail, the pipeline should fail and the deployment should not proceed
            // However, things to consider is that the database schema should be backward compatible, so that the application can still run even if the migrations fail to avoid downtime (probably a good idea to have a fallback plan)
            // This is a good practice to ensure that the database schema is always in sync with the application code (opinionated)
            await using var serviceScope = app.Services.CreateAsyncScope();
            await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
using RetailPortal.Aspire.ServiceDefaults;
using RetailPortal.Data.Db.Context;
using RetailPortal.MigrationService;
using RetailPortal.Model.Constants;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

builder.AddServiceDefaults(configuration);

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Appsettings.PostgresSQLConnectionName));

builder.AddNpgsqlDbContext<ApplicationDbContext>(Appsettings.PostgresSQLConnectionName);

var host = builder.Build();

await host.RunAsync();

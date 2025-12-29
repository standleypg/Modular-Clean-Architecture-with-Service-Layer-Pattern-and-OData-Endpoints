using Microsoft.AspNetCore.OData;
using RetailPortal.Api;
using RetailPortal.Aspire.ServiceDefaults;
using RetailPortal.Data;
using RetailPortal.Service;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.AddServiceDefaults(configuration);

builder.Services
    .AddServiceCollections()
    .AddApplication()
    .AddData(configuration);

builder.Services.AddControllers().AddOData(options =>
    options.Filter().Select().Expand().OrderBy().Count().SetMaxTop(1000)
);

var app = builder.Build();

app.MapDefaultEndpoints();

await app.AddWebApplication();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
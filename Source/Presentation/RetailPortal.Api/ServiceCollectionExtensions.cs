using Asp.Versioning;
using Mapster;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace RetailPortal.Api;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServiceCollections()
        {
            services
                .AddOpenApi()
                .AddMapping();
            return services;
        }

        private void AddMapping()
        {
            services.AddMapster();
        }

        private IServiceCollection AddOpenApi()
        {
            services.AddOpenApi("v0", options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info = new OpenApiInfo { Title = "Retail Portal API - v0.0", Version = "0.0" };

                    return Task.CompletedTask;
                });
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(0, 0);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
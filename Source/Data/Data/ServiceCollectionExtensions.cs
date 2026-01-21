using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using RetailPortal.Data.Auth;
using RetailPortal.Data.Db.Context;
using RetailPortal.Data.Db.Repositories;
using RetailPortal.Data.Db.UnitOfWork;
using RetailPortal.Data.Services;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.DataFacade.Services;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace RetailPortal.Data;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddData(IConfiguration configuration)
        {
            services.AddDbContext(configuration);

            services
                .AddConfigurationBinding(configuration)
                .AddAuth(configuration);

            services
                .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                .AddSingleton<IPasswordHasher, PasswordHasher>()
                .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            services
                .AddScoped<IReadStore, ReadStore>()
                .AddScoped(typeof(IAggregateRepository<>), typeof(AggregateRepository<>))
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private void AddDbContext(IConfiguration configuration)
        {
            services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("RetailPortalDb"));
            });
        }

        private void AddAuth(IConfiguration configuration)
        {
            var jwtOptions = services.BuildServiceProvider().GetRequiredService<IOptions<Appsettings.JwtSettings>>().Value;
            var googleOptions = services.BuildServiceProvider().GetRequiredService<IOptions<Appsettings.GoogleSettings>>().Value;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = TokenProvider.RetailPortalApp.ToString(),
                        ValidAudience = TokenProvider.RetailPortalApp.ToString(),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    };
                })
                .AddJwtBearer(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = googleOptions.Authority;
                    options.Audience = googleOptions.ClientId;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = googleOptions.Authority,
                        ValidateAudience = true,
                        ValidAudience = googleOptions.ClientId,
                        ValidateLifetime = true,
                    };

                    JwtEventsHandler(options, TokenProvider.Google.ToString());
                })
                .AddMicrosoftIdentityWebApi(options =>
                {
                    configuration.Bind(Appsettings.AzureAdSettings.SectionName, options);

                    JwtEventsHandler(options, TokenProvider.Microsoft.ToString());
                }, options =>
                {
                    configuration.Bind(Appsettings.AzureAdSettings.SectionName, options);
                }, Appsettings.AzureAdSettings.JwtBearerScheme);

            services.AddAuthorization();

            return;

            static void JwtEventsHandler(JwtBearerOptions options, string provider)
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // if Request.Header.Token is not from Google, skip the entire process
                        var issuer = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ')[1];
                        var token = new JwtSecurityToken(issuer);
                        var isTrue = token.Claims.Select(c => c.Issuer).First().Contains(provider, StringComparison.OrdinalIgnoreCase);

                        if(!isTrue)
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    },
                };
            }
        }

        private IServiceCollection AddConfigurationBinding(IConfiguration configuration)
        {
            BindAndAdd<Appsettings.JwtSettings>(Appsettings.JwtSettings.SectionName);
            BindAndAdd<Appsettings.GoogleSettings>(Appsettings.GoogleSettings.SectionName);
            BindAndAdd<Appsettings.AzureAdSettings>(Appsettings.AzureAdSettings.SectionName);

            return services;

            void BindAndAdd<TSettings>(string sectionName) where TSettings : class, new()
            {
                var settings = new TSettings();
                configuration.Bind(sectionName, settings);
                services.AddSingleton(Options.Create(settings));
            }
        }
    }
}
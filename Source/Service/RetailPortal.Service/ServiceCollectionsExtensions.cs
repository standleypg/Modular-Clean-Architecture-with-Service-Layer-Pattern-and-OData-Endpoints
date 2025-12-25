using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using RetailPortal.Service.Services.Auth;
using RetailPortal.Service.Services.Product;
using RetailPortal.Service.Services.Role;
using RetailPortal.Service.Validators;
using RetailPortal.ServiceFacade.Auth;
using RetailPortal.ServiceFacade.Product;
using RetailPortal.ServiceFacade.Role;
using System.Reflection;

namespace RetailPortal.Service;

public static class ServiceCollectionsExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<ITokenExchangeService, TokenExchangeService>()
                .AddScoped<ILoginService, LoginService>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IRegisterService, RegisterService>();

            return services;
        }
    }
}
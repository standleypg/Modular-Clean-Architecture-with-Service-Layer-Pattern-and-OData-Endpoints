using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RetailPortal.Service.Services.Auth;
using RetailPortal.Service.Services.Product;
using RetailPortal.Service.Validators.Common;
using RetailPortal.ServiceFacade.Auth;
using RetailPortal.ServiceFacade.Product;
using System.Reflection;

using IInternalValidator = RetailPortal.ServiceFacade.Validator.Common.IValidator;

namespace RetailPortal.Service;

public static class ServiceCollectionsExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddScoped<ITokenExchangeService, TokenExchangeService>()
                .AddScoped<ILoginService, LoginService>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IRegisterService, RegisterService>()

                .AddTransient<IInternalValidator, Validator>();

            return services;
        }
    }
}
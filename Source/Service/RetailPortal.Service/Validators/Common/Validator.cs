using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RetailPortal.Model.DTOs.Common;
using IInternaLValidator = RetailPortal.ServiceFacade.Validator.Common.IValidator;

namespace RetailPortal.Service.Validators.Common;

public class Validator(IServiceProvider serviceProvider): IInternaLValidator
{
    public async Task<Result<TRequest, string>> ValidateAndExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>();

        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );

                return Result<TRequest, string>.Failure(errors);
            }
        }

        return Result<TRequest, string>.Success(request);
    }
}
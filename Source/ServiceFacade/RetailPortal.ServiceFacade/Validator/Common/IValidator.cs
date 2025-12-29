using RetailPortal.Model.DTOs.Common;

namespace RetailPortal.ServiceFacade.Validator.Common;

public interface IValidator
{
    Task<Result<TRequest, string>> ValidateAndExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken);
}
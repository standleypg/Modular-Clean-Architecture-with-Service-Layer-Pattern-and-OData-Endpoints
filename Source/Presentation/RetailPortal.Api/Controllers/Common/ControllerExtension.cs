using Microsoft.AspNetCore.Mvc;
using RetailPortal.Model.DTOs.Common;

namespace RetailPortal.Api.Controllers.Common;

public static class ControllerExtension
{
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    extension<T>(Result<T, string> result)
    {
        /// <summary>
        /// Smart conversion that automatically routes errors to detail or extensions based on error structure
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="failureStatusCode"></param>
        /// <returns></returns>
        public ActionResult Match(ControllerBase controller,
            int failureStatusCode = StatusCodes.Status400BadRequest)
        {
            if (result.IsSuccess)
            {
                return controller.Ok(result.Value);
            }

            if (result.Errors.Count == 1)
            {
                var (_, errors) = result.Errors.First();

                if (errors.Count == 1)
                {
                    return controller.Problem(
                        detail: errors[0],
                        statusCode: failureStatusCode
                    );
                }
            }

            return controller.Problem(
                statusCode: failureStatusCode,
                extensions: new Dictionary<string, object?> { { Result<T, string>.DefaultErrorKey, result.Errors } }
            );
        }
    }

    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    extension<T, TError>(Result<T, TError> result)
    {
        /// <summary>
        /// Matches on the Result with dictionary errors
        /// </summary>
        public TResult Match<TResult>(Func<T, TResult> onSuccess,
            Func<IReadOnlyDictionary<string, List<TError>>, TResult> onFailure)
        {
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Errors);
        }
    }
}
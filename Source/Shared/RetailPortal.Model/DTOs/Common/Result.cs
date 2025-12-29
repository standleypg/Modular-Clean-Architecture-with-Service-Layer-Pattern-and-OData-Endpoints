namespace RetailPortal.Model.DTOs.Common;

public record Result<T, TError>
{
    public const string DefaultErrorKey = "errors";

    public bool IsSuccess => this._errors == null || this._errors.Count == 0;

    private T? _value;
    private readonly Dictionary<string, List<TError>>? _errors;

    public T Value
    {
        get => this.IsSuccess
            ? this._value!
            : throw new InvalidOperationException("No value present for failed result.");
        private set => this._value = value;
    }

    public IReadOnlyDictionary<string, List<TError>> Errors =>
        !this.IsSuccess
            ? this._errors!
            : throw new InvalidOperationException("No errors present for successful result.");

    private Result(T? value, Dictionary<string, List<TError>>? errors) =>
        (this._value, this._errors) = (value, errors);

    /// <summary>
    /// Success factory method
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<T, TError> Success(T value) => new(value, null);

    /// <summary>
    /// Single error (uses default key)
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<T, TError> Failure(TError error) =>
        Failure(DefaultErrorKey, error);

    /// <summary>
    /// Single error with custom key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<T, TError> Failure(string key, TError error) =>
        new(default, new Dictionary<string, List<TError>> { { key, [error] } });

    /// <summary>
    /// Multiple errors (uses default key)
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static Result<T, TError> Failure(params TError[] errors) =>
        Failure(DefaultErrorKey, errors);

    /// <summary>
    /// Multiple errors (uses default key)
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static Result<T, TError> Failure(IEnumerable<TError> errors) =>
        Failure(DefaultErrorKey, errors);

    /// <summary>
    /// Multiple errors with custom key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static Result<T, TError> Failure(string key, IEnumerable<TError> errors) =>
        new(default, new Dictionary<string, List<TError>> { { key, errors.ToList() } });

    /// <summary>
    /// Dictionary of errors (from validation)
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static Result<T, TError> Failure(Dictionary<string, List<TError>> errors) =>
        new(default, errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

    /// <summary>
    /// Dictionary of errors (IReadOnlyDictionary)
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static Result<T, TError> Failure(IReadOnlyDictionary<string, List<TError>> errors) =>
        new(default, errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

    /// <summary>
    /// Implicit conversion
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Result<T, TError>(T value) => Success(value);

    /// <summary>
    /// Creates a Result from an async function
    /// </summary>
    /// <param name="asyncFunc"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<Result<T, TError>> CreateAsync(Func<Task<T>> asyncFunc)
    {
        try
        {
            var result = await asyncFunc();
            return Success(result);
        }
        catch (Exception ex)
        {
            // Only works if TError is string
            if (typeof(TError) != typeof(string))
            {
                throw new InvalidOperationException("CreateAsync without error mapper requires TError to be string");
            }

            return Failure((TError)(object)ex.Message);
        }
    }
}

public static class ResultExtensions
{

    /// <param name="result"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    extension<TSource, TDestination>(Result<TSource, string> result)
    {
        /// <summary>
        /// Converts a failure Result to another Result type
        /// </summary>
        public Result<TDestination, string> ConvertFailure()
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Cannot convert a successful result.");
            }

            return Result<TDestination, string>.Failure(result.Errors);
        }
    }
}
namespace RetailPortal.Model.DTOs.Common;

public class Result<T, TError>
{
    public bool IsSuccess => this._error == null;
    private T? _value;
    private TError? _error;

    public T Value
    {
        get => this.IsSuccess
            ? this._value!
            : throw new InvalidOperationException("No value present for failed result.");
        private set => this._value = value;
    }

    public TError Error
    {
        get => !this.IsSuccess
            ? this._error!
            : throw new InvalidOperationException("No error present for successful result.");
        private set => this._error = value;
    }

    private Result(T? value, TError? error) =>
        (this._value, this._error) = (value, error);

    public static Result<T, TError> Success(T value) => new(value, default);

    public static Result<T, TError> Failure(TError error) => new(default, error);
}
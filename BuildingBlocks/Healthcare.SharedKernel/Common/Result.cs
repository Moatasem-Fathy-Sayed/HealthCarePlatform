namespace Healthcare.SharedKernel.Common;

// Generic result wrapper to handle success/failure state without throwing exceptions
public class Result<TValue>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TValue? Value { get; }
    public Error Error { get; }

    protected internal Result(TValue? value, bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Invalid result state.");
        }

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    // Factory method for successful result
    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);

    // Factory method for failed result
    public static Result<TValue> Failure(Error error) => new(default, false, error);
}
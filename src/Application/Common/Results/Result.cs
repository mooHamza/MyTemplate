
namespace FullTemplate.Application.Common.Results;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors, ErrorKind? errorKind = null)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
        ErrorKind = errorKind;
    }

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }
    public ErrorKind? ErrorKind { get; init; }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(ErrorKind? errorKind, params IEnumerable<string> errors )
    {
        return new Result(false, errors, errorKind);
    }
}
public class Result<T> : Result
{
    internal Result(bool succeeded, T? value, IEnumerable<string> errors, ErrorKind? errorKind = null)
        : base(succeeded, errors, errorKind)
    {
        Value = value;
    }

    public T? Value { get; init; }

    public static Result<T> Success(T value) => new (true, value, Array.Empty<string>());

    public new static Result<T> Failure(ErrorKind? errorKind, params IEnumerable<string> errors) => new(false, default, errors, errorKind);
}

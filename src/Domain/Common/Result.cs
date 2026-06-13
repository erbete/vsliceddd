using System;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Common;

public enum ErrorCode { Unspecified = 0, None, NotFound, Conflict, Validation }

public readonly record struct ResultError(ErrorCode Code, string Message)
{
    public static readonly ResultError None = new(ErrorCode.None, "");
    public static ResultError NotFound(string message) => new(ErrorCode.NotFound, message);
    public static ResultError Conflict(string message) => new(ErrorCode.Conflict, message);
    public static ResultError Validation(string message) => new(ErrorCode.Validation, message);
}

public readonly struct Failure(ResultError error)
{
    public ResultError Error { get; } = error;
}

public readonly struct Result
{
    public ResultError Error { get; }
    public bool IsSuccess => Error.Code == ErrorCode.None;

    private Result(ResultError error) => Error = error;

    public static Result Success() => new(ResultError.None);
    public static Failure Fail(ResultError error) => new(error);   // returns Failure, not Result

    public static implicit operator Result(Failure f) => new(f.Error);
}

public readonly struct Result<T>
{
    private readonly T? _value;
    public ResultError Error { get; }
    public bool IsSuccess => Error.Code == ErrorCode.None;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"No value on failed result ({Error.Code}: {Error.Message}).");

    private Result(T? value, ResultError error)
    {
        _value = value;
        Error = error;
    }

    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Canonical Result factory idiom.")]
    public static Result<T> Success(T value) => new(value, ResultError.None);

    public static implicit operator Result<T>(Failure f) => new(default, f.Error);
}
using Sarhne.Application.Common.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Results;


public class Result
{
    protected Result(bool isSuccess, IReadOnlyList<Error> errors)
    {
        if (isSuccess && errors.Count > 0)
            throw new InvalidOperationException();

        if (!isSuccess && errors.Count == 0)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public IReadOnlyList<Error> Errors { get; }

    public static Result Success()
        => new(true, []);

    public static Result Failure(Error error)
        => new(false, [error]);

    public static Result Failure(IEnumerable<Error> errors)
        => new(false, errors.ToList());

    public static implicit operator Result(Error error)
        => Failure(error);

    public static implicit operator Result(List<Error> errors)
        => Failure(errors);

    public static implicit operator Result(Error[] errors)
        => Failure(errors);
}

//_______________________________________________________________

public class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
        : base(true, [])
    {
        _value = value;
    }

    private Result(IReadOnlyList<Error> errors)
        : base(false, errors)
    {
    }

    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access value of a failed result.");

    public static Result<T> Success(T value)
        => new(value);

    public new static Result<T> Failure(Error error)
        => new([error]);

    public new static Result<T> Failure(IEnumerable<Error> errors)
        => new(errors.ToList());

    public static implicit operator Result<T>(T value)
        => Success(value);

    public static implicit operator Result<T>(Error error)
        => Failure(error);

    public static implicit operator Result<T>(List<Error> errors)
        => Failure(errors);

    public static implicit operator Result<T>(Error[] errors)
        => Failure(errors);
}
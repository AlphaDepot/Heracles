using System.Diagnostics.CodeAnalysis;
using Heracles.Domain.Abstractions.Errors;

namespace Heracles.Domain.Abstractions.Responses;

public class ServiceResponse
{
    protected ServiceResponse(bool isSuccess, Error error, int statusCode)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }


    public int StatusCode { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    // expose static methods to create instances of ServiceResponse
    public static ServiceResponse Success() => new(true, Error.None, statusCode: 200);


    public static ServiceResponse<TValue> Success<TValue>(TValue value, int statusCode = 200) =>
        new(value, true, Error.None, statusCode: statusCode);


    public static ServiceResponse Failure(Error error, int statusCode = 400) => new(false, error, statusCode: statusCode);

    public static ServiceResponse<TValue> Failure<TValue>(Error error, int statusCode = 400) =>
        new(default, false, error, statusCode: statusCode);
}

public class ServiceResponse<TValue> : ServiceResponse
{
    private readonly TValue? _value;

    protected internal ServiceResponse(TValue? value, bool isSuccess, Error error, int statusCode)
        : base(isSuccess, error, statusCode)
    {
        _value = value;
    }

    /// <summary>
    ///  If the serviceResponse is a success, this will return the value, otherwise it will throw an exception
    /// </summary>
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure serviceResponse can't be accessed.");

    /// <summary>
    ///  If the serviceResponse is a success, this will return the value, otherwise it will return the default value
    /// </summary>
    /// <param name="value"> The value to return if the serviceResponse is a failure</param>
    /// <returns> The value if the serviceResponse is a success, otherwise the default value</returns>
    public static implicit operator ServiceResponse<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
using System.Diagnostics.CodeAnalysis;
using Heracles.Domain.Abstractions.Errors;

namespace Heracles.Domain.Abstractions.Responses;

public class DomainResponse
{
    protected DomainResponse(bool isSuccess, Error error, int statusCode)
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

    // expose static methods to create instances of DomainResponse
    public static DomainResponse Success() => new(true, Error.None, statusCode: 200);


    public static DomainResponse<TValue> Success<TValue>(TValue value, int statusCode = 200) =>
        new(value, true, Error.None, statusCode: statusCode);


    public static DomainResponse Failure(Error error, int statusCode = 400) => new(false, error, statusCode: statusCode);

    public static DomainResponse<TValue> Failure<TValue>(Error error, int statusCode = 400) =>
        new(default, false, error, statusCode: statusCode);
}

public class DomainResponse<TValue> : DomainResponse
{
    private readonly TValue? _value;

    protected internal DomainResponse(TValue? value, bool isSuccess, Error error, int statusCode)
        : base(isSuccess, error, statusCode)
    {
        _value = value;
    }

    /// <summary>
    ///  If the domainResponse is a success, this will return the value, otherwise it will throw an exception
    /// </summary>
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure domainResponse can't be accessed.");

    /// <summary>
    ///  If the domainResponse is a success, this will return the value, otherwise it will return the default value
    /// </summary>
    /// <param name="value"> The value to return if the domainResponse is a failure</param>
    /// <returns> The value if the domainResponse is a success, otherwise the default value</returns>
    public static implicit operator DomainResponse<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
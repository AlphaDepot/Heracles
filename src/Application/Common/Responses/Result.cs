using Application.Common.Errors;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Responses;

/// <summary>
///     Represents the result of an operation, indicating success or failure, and containing an error if applicable.
/// </summary>
public class Result
{
	/// <summary>
	///     Initializes a new instance of the <see cref="Result" /> class.
	/// </summary>
	/// <param name="isSuccess">Indicates whether the operation was successful.</param>
	/// <param name="error">The error associated with the operation, if any.</param>
	/// <exception cref="ArgumentException">Thrown when the error type is invalid.</exception>
	protected Result(bool isSuccess, Error? error)
	{
		switch (isSuccess)
		{
			case true when error != Error.None:
			case false when error == Error.None:
				throw new ArgumentException(ErrorCodes.InvalidErrorType, nameof(error));
		}

		IsSuccess = isSuccess;
		Error = error;
		StatusCode = error?.StatusCode ?? StatusCodes.Status200OK;
	}


	/// <summary>
	///     HTTP status code associated with the result.
	/// </summary>
	public int StatusCode { get; }

	/// <summary>
	///     Whether the operation was successful.
	/// </summary>
	public bool IsSuccess { get; }

	/// <summary>
	///     Indicates whether the operation was a failure.
	///     Automatically generated by the compiler based on the value of <see cref="IsSuccess" />.
	/// </summary>
	public bool IsFailure => !IsSuccess;

	/// <summary>
	///     Gets the error associated with the operation, if any. <see cref="Error" />.
	/// </summary>
	public Error? Error { get; }

	/// <summary>
	///     List of errors associated with the operation, if any. <see cref="Error" />
	/// </summary>
	public Error[]? Errors { get; set; }


	// expose static methods to create instances of Result
	/// <summary>
	///     Creates a successful result with no error
	/// </summary>
	/// <returns>A successful <see cref="Result" /> instance.</returns>
	public static Result Success()
	{
		return new Result(true, Error.None);
	}

	/// <summary>
	///     Creates a successful result with a value.
	///     This is a static method for convenience.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="value">The value associated with the successful result.</param>
	/// <returns>A successful <see cref="Result{TValue}" /> instance.</returns>
	public static Result<TValue> Success<TValue>(TValue value)
	{
		return new Result<TValue>(value, true, Error.None);
	}

	/// <summary>
	///     Creates a failed result.
	///     This is a static method for convenience.
	/// </summary>
	/// <param name="error">The error associated with the failed result.</param>
	/// <returns>A failed <see cref="Result" /> instance.</returns>
	public static Result Failure(Error error)
	{
		return new Result(false, error);
	}

	/// <summary>
	///     Creates a failed result with a value.
	///     This is a static method for convenience.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="error">The error associated with the failed result.</param>
	/// <returns>A failed <see cref="Result{TValue}" /> instance.</returns>
	public static Result<TValue> Failure<TValue>(Error error)
	{
		return new Result<TValue>(default, false, error);
	}
}

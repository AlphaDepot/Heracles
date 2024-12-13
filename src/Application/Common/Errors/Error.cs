using Application.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Errors;

/// <summary>
///     Represents an error with a type, status code, and optional description.
/// </summary>
/// <param name="Type">The type of the error.</param>
/// <param name="StatusCode">The HTTP status code associated with the error. Default is 200 OK.</param>
/// <param name="Description">The description of the error, if any.</param>
public record Error(string Type, int StatusCode = StatusCodes.Status200OK, string? Description = null)
{
	/// <summary>
	///     Static method representing no error.
	/// </summary>
	public static readonly Error None = new(string.Empty, StatusCodes.Status200OK, string.Empty);

	/// <summary>
	///     Static method representing an invalid error type.
	/// </summary>
	public static readonly Error NullValue = new(ErrorCodes.NullValue, StatusCodes.Status500InternalServerError,
		"Null value was provided");

	/// <summary>
	///     A static method to implicitly convert <see cref="Error" /> to a <see cref="Result" />.
	/// </summary>
	/// <param name="error">The error to convert.</param>
	public static implicit operator Result(Error error)
	{
		return Result.Failure(error);
	}

	/// <summary>
	///     A static method to the current <see cref="Error" /> to a <see cref="Result" />.
	/// </summary>
	/// <returns>A <see cref="Result" /> representing the failure.</returns>
	public Result ToResult()
	{
		return Result.Failure(this);
	}
}

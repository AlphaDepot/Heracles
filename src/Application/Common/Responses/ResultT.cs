using System.Diagnostics.CodeAnalysis;
using Application.Common.Errors;

namespace Application.Common.Responses;

/// <inheritdoc cref="Result" />
/// <typeparam name="TValue">The type of the value associated with the result.</typeparam>
public class Result<TValue>(TValue? value, bool isSuccess, Error error) : Result(isSuccess, error)
{
	/// <summary>
	///     If the serviceResponse is a success, this will return the value, otherwise it will return null
	/// </summary>
	public TValue? Value { get; } = value;

	/// <summary>
	///     If the serviceResponse is a success, this will return the value, otherwise it will return the default value
	/// </summary>
	/// <param name="value"> The value to return if the serviceResponse is a failure</param>
	/// <returns> The value if the response is a success, otherwise the default value</returns>
	public static implicit operator Result<TValue>(TValue? value)
	{
		return value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
	}
}

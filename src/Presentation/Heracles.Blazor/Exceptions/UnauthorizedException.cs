namespace Heracles.Blazor.Exceptions;

/// <summary>
///     Represents an exception that is thrown when a user attempts to access a resource
///     without proper authentication.
/// </summary>
/// <remarks>
///     This exception is intended to indicate that the user must be logged in to perform
///     the requested operation or access the specified resource.
/// </remarks>
public sealed class UnauthorizedException : Exception
{
	public UnauthorizedException()
		: base("You must be logged in to access this resource.")
	{
	}
}

namespace Heracles.Blazor.Exceptions;

/// <summary>
///     Represents an exception that is thrown when an operation is attempted
///     without sufficient permissions to access the requested resource.
/// </summary>
public sealed class ForbiddenException : Exception
{
	public ForbiddenException()
		: base("You do not have permission to access this resource.")
	{
	}
}

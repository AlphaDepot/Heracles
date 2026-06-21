namespace Heracles.Blazor.Exceptions;

/// <summary>
///     Represents an exception that is thrown when a requested resource or page is not found.
/// </summary>
/// <remarks>
///     This exception is typically used to signal a "not found" error state in the application.
///     It can be thrown in scenarios where a requested entity does not exist or cannot be located
///     within the context of the operation.
/// </remarks>
public sealed class NotFoundException : Exception
{
	public NotFoundException()
		: base("The requested page was not found.")
	{
	}
}

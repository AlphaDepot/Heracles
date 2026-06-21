namespace Heracles.Blazor.Exceptions;

/// <summary>
///     Represents an exception that is thrown when an operation fails
///     to complete successfully due to an unrecoverable error or invalid state.
/// </summary>
/// <remarks>
///     This exception can be used to indicate failure during
///     an operation, is often meant to be caught by the OperationBoundary Component.
/// </remarks>
public sealed class OperationFailedException : Exception
{
	public OperationFailedException(string message)
		: base(message)
	{
	}
}

using Microsoft.AspNetCore.Http;

namespace Application.Common.Errors;

/// <summary>
///     Represents specific error types.
///     Used to return specific error types and keep the error types and their messages mostly consistent.
/// </summary>
public static class ErrorTypes
{
	// Entity errors
	/// <summary>
	///     Returns a null value error.
	///     Used when a null value is found. Not to be confused with NotFound.
	/// </summary>
	public static Error NullValue =>
		new(ErrorCodes.NullValue, StatusCodes.Status400BadRequest, ErrorMessages.NullValue);

	/// <summary>
	///     Returns a not found error.
	///     Used when an entity is not found.
	///     Mostly used when an entity with the provided id is not found.
	/// </summary>
	public static Error NotFound =>
		new(ErrorCodes.NotFound, StatusCodes.Status404NotFound, ErrorMessages.NotFound);


	/// <summary>
	///     Returns a naming conflict error.
	///     Used when a naming conflict occurs.
	///     // Mostly used when an entity with the same name property already exists.
	/// </summary>
	public static Error NamingConflict =>
		new(ErrorCodes.NamingConflict, StatusCodes.Status400BadRequest, ErrorMessages.NamingConflict);

	/// <summary>
	///     Returns an invalid request error.
	///     This is the most generic error and is used when a request is invalid.
	///     And no other error message has been created for the given scenario
	/// </summary>
	public static Error BadRequest =>
		new(ErrorCodes.BadRequest, StatusCodes.Status400BadRequest, ErrorMessages.InvalidRequest);

	/// <summary>
	///     Returns a duplicate entry error.
	///     This is used when a duplicate entry is found and no entity name is provided.
	/// </summary>
	public static Error DuplicateEntry =>
		new(ErrorCodes.DuplicateEntry, StatusCodes.Status409Conflict, ErrorMessages.DuplicateEntry);

	// Users errors
	/// <summary>
	///     Returns an incomplete user claims error.
	///     This is used when a user's claims are incomplete or missing any required claims.
	/// </summary>
	public static Error IncompleteUserClaims =>
		new(ErrorCodes.IncompleteUserClaims, StatusCodes.Status400BadRequest, ErrorMessages.IncompleteUserClaims);

	/// <summary>
	///     Returns an unauthorized error.
	/// </summary>
	public static Error Unauthorized =>
		new(ErrorCodes.Unauthorized, StatusCodes.Status401Unauthorized, ErrorMessages.Unauthorized);

	// Validation errors
	/// <summary>
	///     Used when a validation error occurs.
	///     Mostly used with fluent validation.
	/// </summary>
	public static Error Validation =>
		new(ErrorCodes.Validation, StatusCodes.Status400BadRequest, ErrorMessages.Validation);

	// Type errors
	/// <summary>
	///     Used when an invalid error type is provided.
	///     Created to be used when an error type is not found.
	///     Design to be used for the Result class error property validation.
	/// </summary>
	public static Error InvalidErrorType =>
		new(ErrorCodes.InvalidErrorType, StatusCodes.Status400BadRequest, ErrorMessages.InvalidErrorType);

	/// <summary>
	///     Used when a concurrency error occurs.
	///     Mostly used when an entity is being updated and the concurrency token does not match the current value.
	/// </summary>
	public static Error ConcurrencyError =>
		new(ErrorCodes.ConcurrencyError, StatusCodes.Status409Conflict, ErrorMessages.ConcurrencyError);


	/// <summary>
	///     Returns a generic database error.
	/// </summary>
	public static Error DatabaseError =>
		new(ErrorCodes.DatabaseError, StatusCodes.Status500InternalServerError, ErrorMessages.DatabaseError);

	/// <summary>
	///     Returns a not found error with the entity name provided.
	///     Helps it differentiate the entity that is being validated when more than one entity is being validated.
	/// </summary>
	/// <param name="entityName"> The name of the entity. </param>
	/// <returns></returns>
	public static Error NotFoundWithEntityName(string entityName)
	{
		return new Error(ErrorCodes.NotFound, StatusCodes.Status404NotFound, $"{entityName} {ErrorMessages.NotFound}");
	}


	/// <summary>
	///     Returns a not found error with a message provided.
	///     The last resort is when a specific message is needed.
	/// </summary>
	/// <param name="message"> The message to be returned with the error. </param>
	/// <returns> A not found error with the provided message. </returns>
	public static Error NotFoundWithMessage(string message)
	{
		return new Error(ErrorCodes.NotFound, StatusCodes.Status404NotFound, message);
	}

	/// <summary>
	///     Same as InvalidRequest but with a message.
	/// </summary>
	/// <param name="message"> The message to be returned with the error. </param>
	/// <returns> An invalid request error with the provided message. </returns>
	public static Error
		BadRequestWithMessage(string message)
	{
		return new Error(ErrorCodes.BadRequest, StatusCodes.Status400BadRequest, message);
	}

	/// <summary>
	///     Returns a duplicate entry error with the entity name and parent entity name if provided.
	///     This is used when a relationship between entities is being created and a duplicate entry is found.
	/// </summary>
	/// <param name="entityName"> The name of the entity. </param>
	/// <param name="parentEntityName"> The name of the parent entity aka the entity this will belong to. </param>
	/// <returns> A duplicate entry error with the entity name and parent entity name if provided. </returns>
	public static Error DuplicateEntryWithEntityNames(string entityName, string? parentEntityName = null)
	{
		return new Error(ErrorCodes.DuplicateEntry, StatusCodes.Status409Conflict,
			$"{entityName} {ErrorMessages.DuplicateEntry} {(parentEntityName != null ? $"for {parentEntityName}" : string.Empty)}");
	}

	/// <summary>
	///     Returns a database error with a message.
	/// </summary>
	/// <param name="message"> The message to be returned with the error. </param>
	/// <returns> A database error with the provided message. </returns>
	public static Error DatabaseErrorWithMessage(string message)
	{
		return new Error(ErrorCodes.DatabaseError, StatusCodes.Status500InternalServerError, message);
	}
}

/// <summary>
///     Represents specific error codes used to build the ErrorTypes class.
/// </summary>
public static class ErrorCodes
{
	// Entity errors
	public const string NullValue = "Error.NullValue";
	public const string NotFound = "Error.NotFound";
	public const string NamingConflict = "Error.NamingConflict";
	public const string BadRequest = "Error.InvalidRequest";
	public const string DuplicateEntry = "Error.DuplicateEntry";

	// Users errors
	public const string IncompleteUserClaims = "Error.IncompleteUserClaims";
	public const string Unauthorized = "Error.Unauthorized";

	// Validation errors
	public const string Validation = "Error.Validation";

	// Type errors
	public const string InvalidErrorType = "Error.InvalidErrorType";
	public const string ConcurrencyError = "Error.Concurrency";

	public const string DatabaseError = "Error.DatabaseError";
}

/// <summary>
///     Represents specific error messages used to build the ErrorTypes class.
/// </summary>
public static class ErrorMessages
{
	// Entity errors
	public const string NullValue = "Value cannot be null.";
	public const string NotFound = "Entity not found.";
	public const string NamingConflict = "Entity with this name already exists.";
	public const string InvalidRequest = "Invalid request.";
	public const string DuplicateEntry = "Entity already exists.";

	// Users errors
	public const string IncompleteUserClaims = "User claims are incomplete.";
	public const string Unauthorized = "Unauthorized.";

	// Validation errors
	public const string Validation = "Validation error.";

	// Type errors
	public const string InvalidErrorType = "Invalid error type.";
	public const string ConcurrencyError = "Concurrency error.";

	public const string DatabaseError = "Database error.";
}

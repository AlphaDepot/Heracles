using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Errors;


/// <summary>
/// Represents an error with type + status code + description,
/// and automatically maps them into FluentResults Metadata.
/// </summary>
public sealed class AppError : IError
{
	public string Type { get; }
	public int StatusCode { get; }
	public string Description { get; }

	public string Message => Description;

	public Dictionary<string, object> Metadata { get; } = new();
	public List<IError> Reasons { get; } = [];

	private AppError(string type, int statusCode, string description)
	{
		Type = type;
		StatusCode = statusCode;
		Description = description;

		// ✅ AUTO-FILL METADATA HERE (this is what you want)
		Metadata["Type"] = type;
		Metadata["StatusCode"] = statusCode;
		Metadata["Description"] = description;
	}

	// =========================
	// Factory
	// =========================

	public static AppError Create(string type, int statusCode, string description)
		=> new(type, statusCode, description);

	// =========================
	// Common errors
	// =========================

	public static AppError NullValue =>
		Create(ErrorCodes.NullValue, StatusCodes.Status500InternalServerError, "Null value was provided");

	// =========================
	// FluentResults helpers
	// =========================

	public static implicit operator Result(AppError appError)
		=> Result.Fail(appError);

	public Result ToResult()
		=> Result.Fail(this);
}

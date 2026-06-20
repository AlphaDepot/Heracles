using FluentResults;

namespace Heracles.Shared.Errors;

/// <summary>
///     Represents an error with type + status code + description,
///     and automatically maps them into FluentResults Metadata.
/// </summary>
public sealed class AppError : IError
{
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

	public string Type { get; }
	public int StatusCode { get; }
	public string Description { get; }

	// =========================
	// Common errors
	// =========================

	public static AppError NullValue =>
		Create(ErrorCodes.NullValue, 500, "Null value was provided");

	public string Message => Description;

	public Dictionary<string, object> Metadata { get; } = new();
	public List<IError> Reasons { get; } = [];

	// =========================
	// Factory
	// =========================

	public static AppError Create(string type, int statusCode, string description)
	{
		return new AppError(type, statusCode, description);
	}

	// =========================
	// FluentResults helpers
	// =========================

	public static implicit operator Result(AppError appError)
	{
		return Result.Fail(appError);
	}

	public Result ToResult()
	{
		return Result.Fail(this);
	}
}

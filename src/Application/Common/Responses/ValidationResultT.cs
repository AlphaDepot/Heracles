using Application.Common.Errors;

namespace Application.Common.Responses;

/// <inheritdoc cref="ValidationResult" />
public class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
	/// <inheritdoc cref="ValidationResult" />
	protected ValidationResult(Error[] errors)
		: base(default!, false, IValidationResult.ValidationError)
	{
		Errors = errors;
	}

	/// <inheritdoc cref="ValidationResult" />
	public static ValidationResult<TValue> WithErrors(Error[] errors)
	{
		return new ValidationResult<TValue>(errors);
	}
}

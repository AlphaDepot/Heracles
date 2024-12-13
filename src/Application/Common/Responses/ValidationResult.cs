using Application.Common.Errors;

namespace Application.Common.Responses;

/// <inheritdoc cref="Result" />
public class ValidationResult : Result, IValidationResult
{
	/// <summary>
	///     Initializes a new instance of the <see cref="ValidationResult" /> class.
	/// </summary>
	/// <param name="errors">The errors associated with the validation result.</param>
	public ValidationResult(Error[] errors)
		: base(false, IValidationResult.ValidationError)
	{
		Errors = errors;
	}

	/// <summary>
	///     Creates a new instance of the <see cref="Result" /> class with the specified error.
	/// </summary>
	/// <param name="errors"> The error associated with the operation, if any.</param>
	/// <returns> A new instance of the <see cref="Result" /> class with the specified error.</returns>
	public static ValidationResult WithErrors(Error[] errors)
	{
		return new ValidationResult(errors);
	}
}

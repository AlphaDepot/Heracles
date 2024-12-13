using Application.Common.Errors;
using Application.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Application.UnitTest.Common.Responses;

public class ValidationResultTest
{
	[Test]
	public void WithErrors_WhenCalled_ReturnsValidationResultWithErrors()
	{
		// Arrange
		var errors = new Error[]
		{
			new("ValidationError", StatusCodes.Status400BadRequest, "message1"),
			new("ValidationError", StatusCodes.Status400BadRequest, "message2")
		};

		// Act
		var result = ValidationResult.WithErrors(errors);

		// Assert
		Assert.That(result.IsSuccess, Is.False);
		Assert.That(result.Errors, Is.EqualTo(errors));
		Assert.That(result.Errors.Length, Is.EqualTo(2));
	}

	[Test]
	public void WithErrors_WhenCalled_ReturnsValidationT_ResultWithErrors()
	{
		// Arrange
		var errors = new Error[]
		{
			new("ValidationError", StatusCodes.Status400BadRequest, "message1"),
			new("ValidationError", StatusCodes.Status400BadRequest, "message2")
		};

		// Act
		var result = ValidationResult<string>.WithErrors(errors);

		// Assert
		Assert.That(result.IsSuccess, Is.False);
		Assert.That(result.Errors, Is.EqualTo(errors));
		Assert.That(result.Errors.Length, Is.EqualTo(2));
	}
}

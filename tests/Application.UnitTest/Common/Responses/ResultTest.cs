using Application.Common.Errors;
using Application.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Application.UnitTest.Common.Responses;

[TestFixture]
public class ResultTest
{
	[Test]
	public void SuccessResult_ShouldReportSuccess()
	{
		var result = Result.Success();
		Assert.That(result.IsSuccess, Is.True);
		Assert.That(result.Error, Is.EqualTo(Error.None));
	}

	[Test]
	public void FailureResult_ShouldReportFailureAndError()
	{
		// Assuming Error constructor takes parameters that describe the error
		// Replace `param1`, `param2`, etc., with actual parameters
		var error = new Error(ErrorCodes.ConcurrencyError, StatusCodes.Status409Conflict,
			ErrorMessages.ConcurrencyError);
		var result = Result.Failure(error);
		Assert.That(result.IsSuccess, Is.False);
		// Adjust the assertion below according to how you can compare errors in your application
		Assert.That(result.Error, Is.EqualTo(error));
	}

	[Test]
	public void SuccessResultT_ShouldReturnCorrectValue()
	{
		var expectedValue = "TestValue";
		var result = Result.Success(expectedValue);
		Assert.That(result.Value, Is.EqualTo(expectedValue));
	}

	[Test]
	public void FailureResultT_ShouldReturnNullWhenAccessingValue()
	{
		// Assuming Error constructor takes parameters that describe the error
		// Replace `param1`, `param2`, etc., with actual parameters
		var error = new Error(ErrorCodes.ConcurrencyError, StatusCodes.Status409Conflict,
			ErrorMessages.ConcurrencyError);
		var result = Result.Failure<string>(error);
		Assert.That(result.Value, Is.Null);
	}

	[Test]
	public void ImplicitOperator_ShouldCreateFailureResultForNullValue()
	{
		string? nullString = null; // Explicitly declare a null string variable
		Result<string> result = nullString; // Use the implicit operator to convert null to Result<string>
		Assert.That(result.IsSuccess, Is.False);
		Assert.That(result.Error, Is.EqualTo(Error.NullValue));
	}

	[Test]
	public void Constructor_ShouldThrowArgumentException_ForInvalidErrorType()
	{
		// Arrange
		var invalidError = Error.None;

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => Result.Failure(invalidError));
		Assert.That(ex.Message, Is.EqualTo($"{ErrorCodes.InvalidErrorType} (Parameter 'error')"));
	}
}

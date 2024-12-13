using Application.Common.Errors;
using Application.Common.Responses;

namespace Application.UnitTest.Common.Errors;

[Category("Errors")]
public class ErrorTests
{
	[Test]
	public void ImplicitOperator_ShouldConvertErrorToFailureResult()
	{
		// Arrange
		var error = new Error("TestError", 400, "Test description");

		// Act
		Result result = error;

		// Assert
		Assert.That(result.IsSuccess, Is.False);
		Assert.That(result.StatusCode, Is.EqualTo(400));
		Assert.That(result.Error, Is.EqualTo(error));
	}

	[Test]
	public void ToResult_ShouldReturnFailureResultWithSameError()
	{
		// Arrange
		var error = new Error("TestError", 400, "Test description");

		// Act
		var result = error.ToResult();

		// Assert
		Assert.That(result.IsSuccess, Is.False);
		Assert.That(result.Error, Is.EqualTo(error));
	}
}

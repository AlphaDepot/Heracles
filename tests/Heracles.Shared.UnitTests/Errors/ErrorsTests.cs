using FluentResults;
using Heracles.Shared.Errors;

namespace Heracles.Shared.UnitTests.Errors;

[Category("Errors")]
public class AppErrorTests
{
	[Test]
	public void ImplicitOperator_ShouldConvertErrorToFailureResult()
	{
		// Arrange
		var error = AppError.Create("TestError", 400, "Test description");

		// Act
		Result result = error;

		// Assert
		Assert.That(result.IsSuccess, Is.False);
		Assert.That(result.Errors, Has.Count.EqualTo(1));
		Assert.That(result.Errors.First().Message, Is.EqualTo(error.Description));
	}

	[Test]
	public void ToResult_ShouldReturnFailureResultWithSameError()
	{
		// Arrange
		var error =AppError.Create("TestError", 400, "Test description");

		// Act
		var result = error.ToResult();

		// Assert
		Assert.That(result.IsSuccess, Is.False);
		Assert.That(result.Errors, Has.Count.EqualTo(1));
		Assert.That(result.Errors.First().Message, Is.EqualTo(error.Description));
	}
}

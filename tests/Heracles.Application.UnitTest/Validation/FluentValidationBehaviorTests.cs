using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Heracles.Application.Validation;
using Mediator;
using Moq;

namespace Heracles.Application.UnitTest.Infrastructure;

[TestFixture]
public class FluentValidationBehaviorTests
{
	[SetUp]
	public void SetUp()
	{
		_validatorMock = new Mock<IValidator<TestRequest>>();

		_behavior = new FluentValidationBehavior<TestRequest, Result>(
			new[] { _validatorMock.Object }
		);
	}

	private Mock<IValidator<TestRequest>> _validatorMock;
	private FluentValidationBehavior<TestRequest, Result> _behavior;

	[Test]
	public async Task Handle_ShouldReturnNext_WhenNoValidationErrors()
	{
		// Arrange
		var request = new TestRequest();

		MessageHandlerDelegate<TestRequest, Result> next =
			(_, _) => ValueTask.FromResult(Result.Ok());

		_validatorMock
			.Setup(v => v.Validate(It.IsAny<TestRequest>()))
			.Returns(new ValidationResult());

		// Act
		var result = await _behavior.Handle(request, next, CancellationToken.None);

		// Assert
		Assert.That(result.IsSuccess, Is.True);
	}

	[Test]
	public async Task Handle_ShouldReturnValidationErrors_WhenValidationFails()
	{
		// Arrange
		var request = new TestRequest();

		var failures = new List<ValidationFailure>
		{
			new("Property", "Error message")
		};

		MessageHandlerDelegate<TestRequest, Result> next =
			(_, _) => ValueTask.FromResult(Result.Ok());

		_validatorMock
			.Setup(v => v.Validate(It.IsAny<TestRequest>()))
			.Returns(new ValidationResult(failures));

		// Act
		var result = await _behavior.Handle(request, next, CancellationToken.None);

		// Assert
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.Count, Is.EqualTo(1));
		Assert.That(result.Errors.First().Message, Is.EqualTo("Error message"));
	}

	// IMPORTANT FIX: must be public so Moq can proxy it
	public class TestRequest : IRequest<Result>
	{
	}
}

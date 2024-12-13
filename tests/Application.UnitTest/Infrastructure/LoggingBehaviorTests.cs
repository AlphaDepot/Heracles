using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Logging;
using FluentValidation.Results;
using MediatR;
using Moq;
using ValidationResult = Application.Common.Responses.ValidationResult;

namespace Application.UnitTest.Infrastructure;

[TestFixture]
public class LoggingBehaviorTests
{
	[SetUp]
	public void SetUp()
	{
		_loggerMock = new Mock<IAppLogger<SampleRequest>>();
		_loggingBehavior = new LoggingBehavior<SampleRequest, Result>(_loggerMock.Object);
		_nextMock = new Mock<RequestHandlerDelegate<Result>>();
	}

	private Mock<IAppLogger<SampleRequest>> _loggerMock;
	private LoggingBehavior<SampleRequest, Result> _loggingBehavior;
	private Mock<RequestHandlerDelegate<Result>> _nextMock;

	[Test]
	public async Task Handle_LogsRequestAndResponse()
	{
		// Arrange
		var request = new SampleRequest();
		var response = Result.Success();
		_nextMock.Setup(next => next()).ReturnsAsync(response);

		// Act
		var result = await _loggingBehavior.Handle(request, _nextMock.Object, CancellationToken.None);

		// Assert
		_loggerMock.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(2));
		Assert.That(result, Is.EqualTo(response));
	}

	[Test]
	public async Task Handle_LogsValidationErrors()
	{
		// Arrange
		var request = new SampleRequest();
		var validationErrors = new List<ValidationFailure>
		{
			new("PropertyName", "Validation error")
		};

		var validationResult =
			Result.Failure(new Error(string.Join(", ", validationErrors.Select(e => e.ErrorMessage)), 400));
		_nextMock.Setup(next => next()).ReturnsAsync(validationResult);

		// Act
		var result = await _loggingBehavior.Handle(request, _nextMock.Object, CancellationToken.None);

		// Assert
		_loggerMock.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		Assert.That(result, Is.EqualTo(validationResult));
	}

	[Test]
	public async Task Handle_LogsWarnings()
	{
		// Arrange
		var request = new SampleRequest();
		var warningResult = Result.Failure(new Error("Warning", 400));
		_nextMock.Setup(next => next()).ReturnsAsync(warningResult);

		// Act
		var result = await _loggingBehavior.Handle(request, _nextMock.Object, CancellationToken.None);

		// Assert
		_loggerMock.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		Assert.That(result, Is.EqualTo(warningResult));
	}

	[Test]
	public async Task Handle_LogsErrors()
	{
		// Arrange
		var request = new SampleRequest();
		var errorResult = Result.Failure(new Error("Error", 500));
		_nextMock.Setup(next => next()).ReturnsAsync(errorResult);

		// Act
		var result = await _loggingBehavior.Handle(request, _nextMock.Object, CancellationToken.None);

		// Assert
		_loggerMock.Verify(logger => logger.LogError(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		Assert.That(result, Is.EqualTo(errorResult));
	}

	[Test]
	public async Task Handle_LogsValidationErrors_WhenValidationResultHasErrors()
	{
		// Arrange
		var request = new SampleRequest();
		var validationErrors = new List<ValidationFailure>
		{
			new("PropertyName", "Validation error")
		};

		var validationResult = ValidationResult.WithErrors(new[]
			{ new Error(string.Join(", ", validationErrors.Select(e => e.ErrorMessage)), 400) });
		_nextMock.Setup(next => next()).ReturnsAsync(validationResult);

		// Act
		var actualResult = await _loggingBehavior.Handle(request, _nextMock.Object, CancellationToken.None);

		// Assert
		_loggerMock.Verify(logger => logger.LogWarning(
			"Validation errors occurred for request {@RequestName} at {@DateTimeUtc}. Error details: {@Errors}",
			typeof(SampleRequest).Name, It.IsAny<DateTime>(), validationResult.Errors), Times.Once);
		Assert.That(actualResult, Is.EqualTo(validationResult));
	}

	public class SampleRequest : IRequest<Result>
	{
	}
}

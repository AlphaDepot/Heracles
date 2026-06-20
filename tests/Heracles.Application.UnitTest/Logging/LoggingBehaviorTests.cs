using FluentResults;
using Heracles.Application.Logging;
using Heracles.Shared.Interfaces;
using Mediator;
using Moq;

namespace Heracles.Application.UnitTest.Infrastructure;

[TestFixture]
public class LoggingBehaviorTests
{
	[SetUp]
	public void SetUp()
	{
		_loggerMock = new Mock<IAppLogger<SampleRequest>>();
		_loggingBehavior = new LoggingBehavior<SampleRequest, Result>(_loggerMock.Object);
	}

	private Mock<IAppLogger<SampleRequest>> _loggerMock;
	private LoggingBehavior<SampleRequest, Result> _loggingBehavior;

	[Test]
	public async Task Handle_LogsRequestAndResponse()
	{
		var request = new SampleRequest();
		var response = Result.Ok();

		MessageHandlerDelegate<SampleRequest, Result> next =
			(_, _) => ValueTask.FromResult(response);

		var result = await _loggingBehavior.Handle(request, next, CancellationToken.None);

		_loggerMock.Verify(
			l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
			Times.Exactly(2));

		Assert.That(result, Is.EqualTo(response));
	}

	[Test]
	public async Task Handle_LogsWarnings_ForClientErrors()
	{
		var request = new SampleRequest();
		var warning = Result.Fail(new Error("Warning").WithMetadata("StatusCode", 400));

		MessageHandlerDelegate<SampleRequest, Result> next =
			(_, _) => ValueTask.FromResult(warning);

		var result = await _loggingBehavior.Handle(request, next, CancellationToken.None);

		_loggerMock.Verify(
			l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
			Times.Once);

		Assert.That(result, Is.EqualTo(warning));
	}

	[Test]
	public async Task Handle_LogsErrors_ForServerErrors()
	{
		var request = new SampleRequest();
		var error = Result.Fail(new Error("Error").WithMetadata("StatusCode", 500));

		MessageHandlerDelegate<SampleRequest, Result> next =
			(_, _) => ValueTask.FromResult(error);

		var result = await _loggingBehavior.Handle(request, next, CancellationToken.None);

		_loggerMock.Verify(
			l => l.LogError(It.IsAny<string>(), It.IsAny<object[]>()),
			Times.Once);

		Assert.That(result, Is.EqualTo(error));
	}

	public class SampleRequest : IRequest<Result>
	{
	}
}

using Application.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTest.Infrastructure;


[TestFixture]
public class GlobalExceptionHandlerTests
{
    private Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private Mock<IProblemDetailsService> _problemDetailsServiceMock;
    private GlobalExceptionHandler _exceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
        _exceptionHandler = new GlobalExceptionHandler(_loggerMock.Object, _problemDetailsServiceMock.Object);
    }

    [Test]
    public async Task TryHandleAsync_LogsError()
    {
        var httpContext = new DefaultHttpContext();
        var exception = new Exception("Test exception");

        await _exceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        _loggerMock.Verify(logger => logger.Log(
	        LogLevel.Error,
	        It.IsAny<EventId>(),
	        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An exception has occurred: Test exception")),
	        exception,
	        It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Once);
    }

    [Test]
    public async Task TryHandleAsync_SetsResponseStatusCodeTo500()
    {
        var httpContext = new DefaultHttpContext();
        var exception = new Exception("Test exception");

        await _exceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task TryHandleAsync_CallsProblemDetailsService()
    {
        var httpContext = new DefaultHttpContext();
        var exception = new Exception("Test exception");

        await _exceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        _problemDetailsServiceMock.Verify(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Once);
    }


    [Test]
    public async Task TryHandleAsync_SetsProblemDetailsCorrectly()
    {
        var httpContext = new DefaultHttpContext();
        var exception = new Exception("Test exception", new Exception("Inner exception message"));

        await _exceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        _problemDetailsServiceMock.Verify(service => service.TryWriteAsync(It.Is<ProblemDetailsContext>(context =>
            context.ProblemDetails.Status == StatusCodes.Status500InternalServerError &&
            context.ProblemDetails.Title == "Server Error" &&
            context.ProblemDetails.Type == "https://tools.ietf.org/html/rfc7231#section-6.6.1" &&
            context.ProblemDetails.Detail == "Inner exception message"
        )), Times.Once);
    }
}

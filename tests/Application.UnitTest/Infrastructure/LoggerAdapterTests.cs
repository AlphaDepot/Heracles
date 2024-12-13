using Application.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTest.Infrastructure;

[TestFixture]
public class LoggerAdapterTests
{
    private Mock<ILoggerFactory> _loggerFactoryMock;
    private Mock<ILogger<SampleClass>> _loggerMock;
    private LoggerAdapter<SampleClass> _loggerAdapter;

    [SetUp]
    public void SetUp()
    {
	    _loggerFactoryMock = new Mock<ILoggerFactory>();
	    _loggerMock = new Mock<ILogger<SampleClass>>();
	    _loggerFactoryMock.Setup(factory => factory.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
	    _loggerAdapter = new LoggerAdapter<SampleClass>(_loggerFactoryMock.Object);
    }

    [Test]
    public void LogInformation_LogsInformationMessage()
    {
	    var message = "Information message";
	    var args = new object[] { "arg1", "arg2" };

	    _loggerAdapter.LogInformation(message, args);

	    _loggerMock.Verify(logger => logger.Log(
		    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
		    It.IsAny<EventId>(),
		    It.Is<It.IsAnyType>((v, t) => v.ToString() == message),
		    It.IsAny<Exception>(),
		    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Test]
    public void LogWarning_LogsWarningMessage()
    {
	    var message = "Warning message";
	    var args = new object[] { "arg1", "arg2" };

	    _loggerAdapter.LogWarning(message, args);

	    _loggerMock.Verify(logger => logger.Log(
		    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
		    It.IsAny<EventId>(),
		    It.Is<It.IsAnyType>((v, t) => v.ToString() == message),
		    It.IsAny<Exception>(),
		    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

	[Test]
	public void LogError_LogsErrorMessage()
	{
	    var message = "Error message";
	    var args = new object[] { "arg1", "arg2" };

	    _loggerAdapter.LogError(message, args);

	    _loggerMock.Verify(logger => logger.Log(
	        It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
	        It.IsAny<EventId>(),
	        It.Is<It.IsAnyType>((v, t) => v.ToString() == message),
	        It.IsAny<Exception>(),
	        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
	}

	public class SampleClass
    {
	    public string? Property { get; set; }
    }
}


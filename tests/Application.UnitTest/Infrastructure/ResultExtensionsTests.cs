
using Application.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTest.Infrastructure;

public class ResultExtensionsTests
{

	[Test]
	public void ToProblemDetails_WhenResultIsSuccessful_ThrowsInvalidOperationException()
	{
		// Arrange
		var result = Application.Common.Responses.Result.Success();

		// Act
		void Act() => result.ToProblemDetails();

		// Assert
		Assert.Throws<InvalidOperationException>(Act);
	}


	[Test]
	public async Task ToProblemDetails_WhenResultHasErrors_ReturnsProblemDetails()
	{
		// Arrange
		var error = Application.Common.Errors.Error.NullValue;
		var result = Application.Common.Responses.Result.Failure(error);
		var httpContext = new DefaultHttpContext();
		var responseStream = new MemoryStream();
		httpContext.Response.Body = responseStream;

		// Set up the service provider
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddLogging();
		var serviceProvider = serviceCollection.BuildServiceProvider();
		httpContext.RequestServices = serviceProvider;

		// Act
		var problemDetailsResult = result.ToProblemDetails();
		await problemDetailsResult.ExecuteAsync(httpContext);

		// Assert
		responseStream.Seek(0, SeekOrigin.Begin);
		var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
		Assert.That(responseBody, Is.Not.Empty);
		Assert.That(httpContext.Response.StatusCode, Is.EqualTo(result.StatusCode));
	}

}

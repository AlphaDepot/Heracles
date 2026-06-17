using Application.Infrastructure.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Application.UnitTest.Infrastructure;

public class ResultExtensionsTests
{
	[Test]
	public void ToProblemDetails_WhenResultIsSuccessful_ThrowsInvalidOperationException()
	{
		var result = Result.Ok("test");

		Assert.Throws<InvalidOperationException>(() =>
		{
			result.ToProblemDetails();
		});
	}

	[Test]
	public async Task ToProblemDetails_WhenResultHasErrors_ReturnsProblemDetails()
	{
		var result = Result.Fail<object>("Something went wrong");

		var httpContext = new DefaultHttpContext();
		var responseStream = new MemoryStream();
		httpContext.Response.Body = responseStream;

		httpContext.RequestServices = new ServiceCollection()
			.AddLogging()
			.BuildServiceProvider();

		var problem = result.ToProblemDetails();
		await problem.ExecuteAsync(httpContext);

		responseStream.Seek(0, SeekOrigin.Begin);
		var body = await new StreamReader(responseStream).ReadToEndAsync();

		Assert.That(body, Is.Not.Empty);
		Assert.That(httpContext.Response.StatusCode, Is.EqualTo(400));
	}
}

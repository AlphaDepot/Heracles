using Application.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Application.Infrastructure.Extensions;

public static class ResultExtensions
{
	public static IResult ToProblemDetails(this Result result)
	{
		if (result.IsSuccess)
		{
			throw new InvalidOperationException("Can't convert successful serviceResponse to problem.");
		}

		var extensions = result.Errors == null
			? new Dictionary<string, object?> { { "errors", new[] { result.Error } } }
			: new Dictionary<string, object?> { { "errors", result.Errors } };

		return Results.Problem(
			statusCode: result.StatusCode,
			title: "One or more validation errors occurred!.",
			type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			extensions: extensions);
	}
}

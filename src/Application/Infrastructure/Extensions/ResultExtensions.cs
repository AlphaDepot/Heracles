using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Infrastructure.Extensions;

public static class ResultExtensions
{
	// Handles Result<T>
	public static IResult ToProblemDetails<T>(this Result<T> result)
	{
		if (result.IsSuccess)
		{
			throw new InvalidOperationException("Can't convert successful serviceResponse to problem.");
		}

		var errors = result.Errors
			.Select(e => new
			{
				e.Message,
				Metadata = e.Metadata
			})
			.ToArray();

		return Results.Problem(
			statusCode: 400,
			title: "One or more errors occurred.",
			type: "https://tools.ietf.org/html/rfc7231#section/6.5.1",
			extensions: new Dictionary<string, object?>
			{
				{ "errors", errors }
			});
	}
}

using FluentResults;
using Heracles.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Heracles.Common.Extensions;

public static class ResultEndpointExtensions
{
	public static IResult ToApiResponse<T>(
		this Result<T> result,
		string version = "1")
	{
		return result.IsSuccess
			? Results.Ok(ApiResponse<T>.Ok(result.Value, version))
			: result.ToProblemDetails();
	}
}

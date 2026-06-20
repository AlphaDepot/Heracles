using System.Text.Json;
using FluentResults;

namespace Heracles.Application.UnitTest.Utilities;

public static class TestLogger
{
	public static void LogFailedResult(this Result result)
	{
		if (result.IsFailed)
		{
			Console.WriteLine(JsonSerializer.Serialize(result.Errors));
		}
	}

	public static void LogFailedResult<T>(this Result<T> result)
	{
		if (result.IsFailed)
		{
			Console.WriteLine(JsonSerializer.Serialize(result.Errors));
		}
	}
}

using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Application.Infrastructure.Middlewares;

/// <summary>
///     Middleware to add the CorrelationId to the Serilog LogContext
/// </summary>
public class RequestLogContextMiddleware
{
	private readonly RequestDelegate _next;

	public RequestLogContextMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public Task InvokeAsync(HttpContext httpContext)
	{
		using (LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier))
		{
			return _next(httpContext);
		}
	}
}

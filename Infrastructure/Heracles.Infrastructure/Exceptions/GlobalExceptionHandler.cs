using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Heracles.Infrastructure.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
  private readonly ILogger<GlobalExceptionHandler> _logger;
   public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
   {
       _logger = logger;
   }

   public async ValueTask<bool> TryHandleAsync(
       HttpContext httpContext,
       Exception exception,
       CancellationToken cancellationToken)
   {
       _logger.LogError(exception, "An exception has occurred: {Message}", exception.Message);

       var problemDetails = new ProblemDetails
       {
           Status = StatusCodes.Status500InternalServerError,
           Title = "Server Error",
           Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
           Detail = exception.InnerException?.Message,
       };

       httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

       await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

       return true;
   }

}
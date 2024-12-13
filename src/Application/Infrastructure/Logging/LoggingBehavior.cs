using Application.Common.Errors;
using Application.Common.Responses;
using MediatR;

namespace Application.Infrastructure.Logging;

/// <summary>
///     A logging behavior that logs the request and response of a request  in a MediatR pipeline.
///     It logs the request at the beginning of the pipeline and logs the response at the end of the pipeline based on the
///     result.
/// </summary>
/// <typeparam name="TRequest"> The request type </typeparam>
/// <typeparam name="TResponse"> The response type </typeparam>
public class LoggingBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : Result
{
	private readonly IAppLogger<TRequest> _logger;

	public LoggingBehavior(IAppLogger<TRequest> logger)
	{
		_logger = logger;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		// Log the request
		_logger.LogInformation("Handling {@RequestName} on {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);

		// Pass the request to the next handler in the pipeline
		var result = await next();

		if (result is ValidationResult { Errors: not null } validationResult)
		{

			_logger.LogWarning(
				"Validation errors occurred for request {@RequestName} at {@DateTimeUtc}. Error details: {@Errors}",
				typeof(TRequest).Name, DateTime.UtcNow, validationResult.Errors);
			return result;
		}

		// Log the response based on the result
		switch (result.IsFailure)
		{
			case true when result.Error is { StatusCode: >= 300 and < 500 }:
				_logger.LogWarning(
					"Warning triggered while handling the request {@RequestName} at {@DateTimeUtc}. Warning details: {@Response}",
					typeof(TRequest).Name, DateTime.UtcNow, result.Error);
				return result;
			case true:
				_logger.LogError(
					"An error occurred while handling the request {@RequestName} at {@DateTimeUtc}. Error details: {@Response}",
					typeof(TRequest).Name, DateTime.UtcNow, result.Error ?? new Error( "An error occurred"));
				return result;
			default:
				_logger.LogInformation("Handled {@RequestName} successfully on {@DateTimeUtc}",
					typeof(TRequest).Name, DateTime.UtcNow);
				return result;
		}
	}
}

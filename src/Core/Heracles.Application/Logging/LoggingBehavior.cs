using FluentResults;
using Heracles.Shared.Interfaces;
using Mediator;

namespace Heracles.Application.Logging;

public class LoggingBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IAppLogger<TRequest> _logger;

	public LoggingBehavior(IAppLogger<TRequest> logger)
	{
		_logger = logger;
	}

	public async ValueTask<TResponse> Handle(
		TRequest request,
		MessageHandlerDelegate<TRequest, TResponse> next,
		CancellationToken cancellationToken)
	{
		_logger.LogInformation(
			"Handling {@RequestName} on {@DateTimeUtc}",
			typeof(TRequest).Name,
			DateTime.UtcNow);

		var result = await next(request, cancellationToken);

		if (result is Result r && r.IsFailed)
		{
			var firstError = r.Errors.FirstOrDefault();

			if (firstError is not null &&
			    firstError.Metadata.TryGetValue("StatusCode", out var statusObj) &&
			    statusObj is int statusCode &&
			    statusCode is >= 300 and < 500)
			{
				_logger.LogWarning(
					"Warning triggered while handling the groupRequest {@RequestName} at {@DateTimeUtc}. Warning details: {@Response}",
					typeof(TRequest).Name,
					DateTime.UtcNow,
					firstError);

				return result;
			}

			if (firstError != null)
			{
				_logger.LogError(
					"An error occurred while handling the groupRequest {@RequestName} at {@DateTimeUtc}. Error details: {@Response}",
					typeof(TRequest).Name,
					DateTime.UtcNow,
					firstError);
			}

			return result;
		}

		_logger.LogInformation(
			"Handled {@RequestName} successfully on {@DateTimeUtc}",
			typeof(TRequest).Name,
			DateTime.UtcNow);

		return result;
	}
}

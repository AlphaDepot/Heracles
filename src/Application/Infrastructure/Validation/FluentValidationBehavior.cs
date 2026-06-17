using Application.Common.Errors;
using FluentResults;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Infrastructure.Validation;

public class FluentValidationBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async ValueTask<TResponse> Handle(
		TRequest request,
		MessageHandlerDelegate<TRequest, TResponse> next,
		CancellationToken cancellationToken)
	{
		var failures = _validators
			.Select(v => v.Validate(request))
			.SelectMany(r => r.Errors)
			.Where(f => f != null)
			.ToList();

		if (failures.Count == 0)
		{
			return await next(request, cancellationToken);
		}

		var errors = failures
			.Select(f => AppError.Create(
				$"Validation.{f.PropertyName}",
				StatusCodes.Status400BadRequest,
				f.ErrorMessage))
			.ToArray();

		return CreateFailure<TResponse>(errors);
	}

	private static TRes CreateFailure<TRes>(AppError[] errors)
	{
		var result = Result.Fail(errors);
		return (TRes)(object)result;
	}
}

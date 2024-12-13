using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Logging;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using ValidationResult = Application.Common.Responses.ValidationResult;

namespace Application.Infrastructure.Validation;

public class FluentValidationBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : Result
{
	private readonly IAppLogger<FluentValidationBehavior<TRequest, TResponse>> _logger;
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators,
		IAppLogger<FluentValidationBehavior<TRequest, TResponse>> logger)
	{
		_validators = validators;
		_logger = logger;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (!_validators.Any())
		{
			return await next();
		}

		var errors = GetValidationErrors(request);

		if (errors.Length == 0)
		{
			return await next();
		}

		return CreateValidationResult<TResponse>(errors);
	}


	/// <summary>
	///     Create a validation result of type TResult with the provided errors
	///     If TResult is Result, return a ValidationResult with the provided errors
	///     Otherwise, create a ValidationResult of the generic type of TResult with the provided errors.
	/// </summary>
	private static TResult CreateValidationResult<TResult>(Error[] errors)
		where TResult : Result
	{
		if (typeof(TResult) == typeof(Result))
		{
			return (ValidationResult.WithErrors(errors) as TResult)!;
		}

		var validationResult = typeof(ValidationResult<>)
			.GetGenericTypeDefinition()
			.MakeGenericType(typeof(TResult).GenericTypeArguments[0])
			.GetMethod(nameof(ValidationResult.WithErrors))!
			.Invoke(null, [errors])!;

		return (TResult)validationResult;
	}


	private Error[] GetValidationErrors(TRequest request)
	{
		// Logic is split into multiple lines for readability

		var validatorsWithFailure = _validators
			.SelectMany(v => v.Validate(request).Errors)
			.Where(validationFailure => validationFailure != null);


		var errors = validatorsWithFailure
			.Select(failure => new Error(
				PropertyNameToErrorType(failure.PropertyName),
				StatusCodes.Status400BadRequest, failure.ErrorMessage))
			.Distinct()
			.ToArray();

		return errors;
	}

	private string PropertyNameToErrorType(string propertyName)
	{
		// If the property name contains a '.', split it and take the second part, otherwise take the property name
		// that way if the property name is something like 'exerciseType.Name' we only take 'Name'
		// Resulting in Error.Validation.Name instead of Error.Validation.exerciseType.Name
		return $"{ErrorCodes.Validation}.{(propertyName.Contains('.') ? propertyName.Split('.')[1] : propertyName)}";
	}
}

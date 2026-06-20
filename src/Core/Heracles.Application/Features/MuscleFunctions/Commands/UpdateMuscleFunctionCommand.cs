using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.MuscleFunctions;

namespace Heracles.Application.Features.MuscleFunctions.Commands;

/// <summary>
///     Updates a <see cref="MuscleFunction" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="MuscleFunction">The <see cref="UpdateMuscleFunctionRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateMuscleFunctionCommand(UpdateMuscleFunctionRequest MuscleFunction, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateMuscleFunctionCommand" />.
/// </summary>
public class UpdateMuscleFunctionCommandValidator : AbstractValidator<UpdateMuscleFunctionCommand>
{
	public UpdateMuscleFunctionCommandValidator()
	{
		RuleFor(x => x.MuscleFunction.Id)
			.NotEmpty().WithMessage("Muscle Function Id is required")
			.GreaterThan(0).WithMessage("Muscle Function Id must be greater than 0");

		RuleFor(x => x.MuscleFunction.Name)
			.NotEmpty().WithMessage("Muscle Function Name is required")
			.Length(3, 50).WithMessage("Muscle Function Name must be between 3 and 50 characters");

		RuleFor(x => x.MuscleFunction.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateMuscleFunctionCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IMuscleFunctionsRepository" />.</param>
public class UpdateMuscleFunctionCommandHandler(IMuscleFunctionsRepository repository)
	: Mediator.IRequestHandler<UpdateMuscleFunctionCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(UpdateMuscleFunctionCommand request, CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		// Apply updated values
		entity.Name = request.MuscleFunction.Name;

		// Regenerate concurrency token
		entity.Concurrency = Guid.NewGuid().ToString();

		await repository.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, MuscleFunction?)> BusinessValidation(
		UpdateMuscleFunctionCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		var existing = await repository.GetByIdAsync(request.MuscleFunction.Id, token);
		if (existing is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		if (existing.Concurrency != request.MuscleFunction.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		var nameExists = await repository.NameInUseAsync(request.MuscleFunction.Name, request.MuscleFunction.Id, token);

		if (nameExists)
		{
			return (Result.Fail<bool>(ErrorTypes.NamingConflict), null);
		}

		return (Result.Ok(true), existing);
	}
}

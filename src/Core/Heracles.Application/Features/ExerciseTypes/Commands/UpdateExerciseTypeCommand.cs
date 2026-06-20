using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.ExerciseTypes;

namespace Heracles.Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Updates an <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="ExerciseType"> The <see cref="UpdateExerciseTypeRequest" /> to update.</param>
/// <param name="IsAdmin"> If true, the command will succeed even if the user is not an admin.</param>
public record UpdateExerciseTypeCommand(UpdateExerciseTypeRequest ExerciseType, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateExerciseTypeCommand" />.
/// </summary>
public class UpdateExerciseTypeCommandValidator : AbstractValidator<UpdateExerciseTypeCommand>
{
	public UpdateExerciseTypeCommandValidator()
	{
		RuleFor(x => x.ExerciseType.Id)
			.GreaterThan(0).WithMessage("Exercise type id is required.");

		RuleFor(x => x.ExerciseType.Concurrency)
			.NotNull().WithMessage("Exercise type concurrency is required.")
			.Length(36).WithMessage("Exercise type concurrency is a guid and must be 36 characters long.");

		RuleFor(x => x.ExerciseType.Name)
			.NotNull().WithMessage("Exercise type name is required.")
			.NotEmpty().WithMessage("Exercise type name is required.")
			.MaximumLength(255).WithMessage("Exercise type name must not exceed 255 characters.");

		RuleFor(x => x.ExerciseType.Description)
			.MaximumLength(1000).WithMessage("Exercise type description must not exceed 1000 characters.");

		RuleFor(x => x.ExerciseType.Images)
			.NotEmpty().WithMessage("Exercise type must have at least one image.");
	}
}

/// <summary>
///     Handles the <see cref="UpdateExerciseTypeCommand" />.
/// </summary>
/// <param name="repository"> The <see cref="IExerciseTypesRepository" />.</param>
public class UpdateExerciseTypeCommandHandler(IExerciseTypesRepository repository)
	: Mediator.IRequestHandler<UpdateExerciseTypeCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(UpdateExerciseTypeCommand request, CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		// Apply updated values
		entity.Name = request.ExerciseType.Name;
		entity.Description = request.ExerciseType.Description;
		entity.Images = request.ExerciseType.Images ?? new List<string>();

		// Regenerate concurrency token
		entity.Concurrency = Guid.NewGuid().ToString();

		await repository.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, ExerciseType?)> BusinessValidation(
		UpdateExerciseTypeCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		var existing = await repository.GetByIdAsync(request.ExerciseType.Id, token);
		if (existing is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		if (existing.Concurrency != request.ExerciseType.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		var nameExists = await repository.NameInUseAsync(request.ExerciseType.Name, request.ExerciseType.Id, token);

		if (nameExists)
		{
			return (Result.Fail<bool>(ErrorTypes.NamingConflict), null);
		}

		return (Result.Ok(true), existing);
	}
}

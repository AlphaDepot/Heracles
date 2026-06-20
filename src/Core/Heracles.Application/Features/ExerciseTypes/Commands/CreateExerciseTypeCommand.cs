using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.ExerciseTypes;

namespace Heracles.Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Creates a new <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="ExerciseType"> The <see cref="CreateExerciseTypeRequest" /> to create.</param>
/// <param name="IsAdmin"> If true, the command will succeed even if the user is not an admin.</param>
public record CreateExerciseTypeCommand(CreateExerciseTypeRequest ExerciseType, bool IsAdmin = true)
	: Mediator.IRequest<Result<int>>;

public class CreateExerciseTypeCommandValidator : AbstractValidator<CreateExerciseTypeCommand>
{
	public CreateExerciseTypeCommandValidator()
	{
		RuleFor(x => x.ExerciseType.Name)
			.NotEmpty().WithMessage("Name is required.")
			.MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

		RuleFor(x => x.ExerciseType.Description)
			.MaximumLength(1000).WithMessage("Exercise type description must not exceed 1000 characters.");

		RuleFor(x => x.ExerciseType.Images)
			.NotEmpty().WithMessage("Exercise type must have at least one image.");
	}
}

/// <summary>
///     Handles the <see cref="CreateExerciseTypeCommand" />.
/// </summary>
/// <param name="repository"> The <see cref="IExerciseTypesRepository" />.</param>
public class CreateExerciseTypeCommandHandler(IExerciseTypesRepository repository)
	: Mediator.IRequestHandler<CreateExerciseTypeCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(CreateExerciseTypeCommand request, CancellationToken token)
	{
		var validation = await BusinessValidation(request, token);
		if (validation.IsFailed)
		{
			return validation;
		}

		var entity = request.ExerciseType.MapCreateRequestToDbEntity();

		await repository.AddAsync(entity, token);
		await repository.SaveChangesAsync(token);

		return Result.Ok(entity.Id);
	}

	private async ValueTask<Result<int>> BusinessValidation(CreateExerciseTypeCommand request, CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		var exists = await repository.ExistsByNameAsync(request.ExerciseType.Name, token);
		if (exists)
		{
			return Result.Fail<int>(ErrorTypes.NamingConflict);
		}

		return Result.Ok(0);
	}
}

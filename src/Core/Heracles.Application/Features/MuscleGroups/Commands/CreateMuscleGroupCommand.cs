using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.MuscleGroups;

namespace Heracles.Application.Features.MuscleGroups.Commands;

/// <summary>
///     Creates a new <see cref="MuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="MuscleGroup">The <see cref="CreateMuscleGroupRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record CreateMuscleGroupCommand(CreateMuscleGroupRequest MuscleGroup, bool IsAdmin = true)
	: Mediator.IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateMuscleGroupCommand" />.
/// </summary>
public class CreateMuscleGroupCommandValidator : AbstractValidator<CreateMuscleGroupCommand>
{
	public CreateMuscleGroupCommandValidator()
	{
		RuleFor(x => x.MuscleGroup.Name)
			.NotEmpty().WithMessage("Name is required")
			.Length(3, 50).WithMessage("Name must be between 3 and 50 characters");
	}
}

/// <summary>
///     Handles the <see cref="CreateMuscleGroupCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IMuscleGroupsRepository" />.</param>
public class CreateMuscleGroupCommandHandler(IMuscleGroupsRepository repository)
	: Mediator.IRequestHandler<CreateMuscleGroupCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(CreateMuscleGroupCommand request, CancellationToken token)
	{
		var validation = await BusinessValidation(request, token);
		if (validation.IsFailed)
		{
			return validation;
		}

		var entity = request.MuscleGroup.MapCreateRequestToDbEntity();

		await repository.AddAsync(entity, token);
		await repository.SaveChangesAsync(token);

		return Result.Ok(entity.Id);
	}

	private async ValueTask<Result<int>> BusinessValidation(CreateMuscleGroupCommand request, CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		var exists = await repository.ExistsByNameAsync(request.MuscleGroup.Name, token);
		if (exists)
		{
			return Result.Fail<int>(ErrorTypes.NamingConflict);
		}

		return Result.Ok(0);
	}
}

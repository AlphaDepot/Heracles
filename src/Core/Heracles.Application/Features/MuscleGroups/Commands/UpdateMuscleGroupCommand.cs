using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.MuscleGroups;

namespace Heracles.Application.Features.MuscleGroups.Commands;

/// <summary>
///     Updates a <see cref="MuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="MuscleGroup">The <see cref="UpdateMuscleGroupRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateMuscleGroupCommand(UpdateMuscleGroupRequest MuscleGroup, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateMuscleGroupCommand" />.
/// </summary>
public class UpdateMuscleGroupCommandValidator : AbstractValidator<UpdateMuscleGroupCommand>
{
	public UpdateMuscleGroupCommandValidator()
	{
		RuleFor(x => x.MuscleGroup.Id)
			.NotEmpty().WithMessage("MuscleGroup Id is required")
			.GreaterThan(0).WithMessage("MuscleGroup Id must be greater than 0");

		RuleFor(x => x.MuscleGroup.Name)
			.NotEmpty().WithMessage("MuscleGroup Name is required")
			.Length(3, 50).WithMessage("MuscleGroup Name must be between 3 and 50 characters");

		RuleFor(x => x.MuscleGroup.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateMuscleGroupCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IMuscleGroupsRepository" />.</param>
public class UpdateMuscleGroupCommandHandler(IMuscleGroupsRepository repository)
	: Mediator.IRequestHandler<UpdateMuscleGroupCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(UpdateMuscleGroupCommand request, CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		// Apply updated values
		entity.Name = request.MuscleGroup.Name;

		// Regenerate concurrency token
		entity.Concurrency = Guid.NewGuid().ToString();

		await repository.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, MuscleGroup?)> BusinessValidation(
		UpdateMuscleGroupCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		var existing = await repository.GetByIdAsync(request.MuscleGroup.Id, token);
		if (existing is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		if (existing.Concurrency != request.MuscleGroup.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		var nameExists = await repository.NameInUseAsync(request.MuscleGroup.Name, request.MuscleGroup.Id, token);

		if (nameExists)
		{
			return (Result.Fail<bool>(ErrorTypes.NamingConflict), null);
		}

		return (Result.Ok(true), existing);
	}
}

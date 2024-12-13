using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MuscleGroups.Commands;

/// <summary>
///     Updates a <see cref="MuscleGroup" />.
/// </summary>
/// <param name="Id"> The Id of the <see cref="MuscleGroup" /> to update.</param>
/// <param name="Name">The new name of the <see cref="MuscleGroup" />.</param>
/// <param name="Concurrency">The concurrency token of the <see cref="MuscleGroup" />.</param>
public record UpdateMuscleGroupRequest(int Id, string Name, string? Concurrency);

/// <summary>
///     Updates a <see cref="MuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="MuscleGroup">The <see cref="UpdateMuscleGroupRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateMuscleGroupCommand(UpdateMuscleGroupRequest MuscleGroup, bool IsAdmin = true)
	: IRequest<Result<bool>>;

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
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class UpdateMuscleGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<UpdateMuscleGroupCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateMuscleGroupCommand request, CancellationToken cancellationToken)
	{
		var (validation, muscleGroup) = await BusinessValidation(request);
		if (validation.IsFailure || muscleGroup == null)
		{
			return validation;
		}


		var updatedMuscleGroup = request.MuscleGroup.MapUpdateRequestToDbEntity(muscleGroup);
		dbContext.Entry(muscleGroup).CurrentValues.SetValues(updatedMuscleGroup);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(
				ErrorTypes.DatabaseErrorWithMessage($"Failed to update Muscle Group {muscleGroup.Id}"));
	}

	private async Task<(Result<bool>, MuscleGroup?)> BusinessValidation(UpdateMuscleGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var existingMuscleGroup = await dbContext.MuscleGroups.FindAsync(request.MuscleGroup.Id);
		if (existingMuscleGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		if (existingMuscleGroup.Concurrency != request.MuscleGroup.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}

		var nameAlreadyExists = await dbContext.MuscleGroups.AnyAsync(x =>
			x.Name == request.MuscleGroup.Name &&
			x.Id != request.MuscleGroup.Id);
		if (nameAlreadyExists)
		{
			return (Result.Failure<bool>(ErrorTypes.NamingConflict), null);
		}

		return (Result.Success(true), existingMuscleGroup);
	}
}

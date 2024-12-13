using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Represents the request to update an <see cref="ExerciseType" />.
/// </summary>
/// <param name="Id"> The id of the <see cref="ExerciseType" /> to update.</param>
/// <param name="Name"> The name of the <see cref="ExerciseType" /> to update.</param>
/// <param name="Concurrency"> The concurrency stamp of the <see cref="ExerciseType" /> to update.</param>
/// <param name="Description"> The description of the <see cref="ExerciseType" /> to update.</param>
/// <param name="ImageUrl"> The image url of the <see cref="ExerciseType" /> to update.</param>
public record UpdateExerciseTypeRequest(
	int Id,
	string Name,
	string? Concurrency,
	string? Description,
	string? ImageUrl);

/// <summary>
///     Updates an <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="ExerciseType"> The <see cref="UpdateExerciseTypeRequest" /> to update.</param>
/// <param name="IsAdmin"> If true, the command will succeed even if the user is not an admin.</param>
public record UpdateExerciseTypeCommand(UpdateExerciseTypeRequest ExerciseType, bool IsAdmin = true)
	: IRequest<Result<bool>>;

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

		RuleFor(x => x.ExerciseType.ImageUrl)
			.MaximumLength(255).WithMessage("Exercise type image url must not exceed 255 characters.");
	}
}

/// <summary>
///     Handles the <see cref="UpdateExerciseTypeCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
public class UpdateExerciseTypeCommandHandler(AppDbContext dbContext)
	: IRequestHandler<UpdateExerciseTypeCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateExerciseTypeCommand request, CancellationToken cancellationToken)
	{
		var (validation, exerciseType) = await BusinessValidation(request);
		if (validation.IsFailure || exerciseType == null)
		{
			return validation;
		}

		var updatedExerciseType = request.ExerciseType.MapUpdateRequestToDbEntity(exerciseType);

		exerciseType.MuscleGroups = updatedExerciseType.MuscleGroups;
		dbContext.Entry(exerciseType).CurrentValues.SetValues(updatedExerciseType);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(ErrorTypes.DatabaseErrorWithMessage($"Failed to update Exercise Type {exerciseType.Id}"));
	}

	private async Task<(Result<bool>, ExerciseType?)> BusinessValidation(UpdateExerciseTypeCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var existingExerciseType = await dbContext.ExerciseTypes.FirstOrDefaultAsync(x => x.Id == request.ExerciseType.Id);
		if (existingExerciseType == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		if (existingExerciseType.Concurrency != request.ExerciseType.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}

		var nameAlreadyExists = await dbContext.ExerciseTypes.AnyAsync(x =>
			x.Name == request.ExerciseType.Name &&
			x.Id != request.ExerciseType.Id);

		if (nameAlreadyExists)
		{
			return (Result.Failure<bool>(ErrorTypes.NamingConflict), null);
		}

		return (true, existingExerciseType);
	}
}

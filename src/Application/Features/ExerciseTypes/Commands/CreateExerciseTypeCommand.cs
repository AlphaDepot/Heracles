using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Creates a new <see cref="ExerciseType" />.
/// </summary>
/// <param name="Name"> The name of the <see cref="ExerciseType" />.</param>
/// <param name="Description"> The description of the <see cref="ExerciseType" />.</param>
/// <param name="ImageUrl"> The image url of the <see cref="ExerciseType" />.</param>
public record CreateExerciseTypeRequest(string Name, string? Description, string? ImageUrl);

/// <summary>
///     Creates a new <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="ExerciseType"> The <see cref="CreateExerciseTypeRequest" /> to create.</param>
/// <param name="IsAdmin"> If true, the command will succeed even if the user is not an admin.</param>
public record CreateExerciseTypeCommand(CreateExerciseTypeRequest ExerciseType, bool IsAdmin = true)
	: IRequest<Result<int>>;

public class CreateExerciseTypeCommandValidator : AbstractValidator<CreateExerciseTypeCommand>
{
	public CreateExerciseTypeCommandValidator()
	{
		RuleFor(x => x.ExerciseType.Name)
			.NotEmpty().WithMessage("Name is required.")
			.MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
		RuleFor(x => x.ExerciseType.Description)
			.MaximumLength(1000).WithMessage("Exercise type description must not exceed 1000 characters.");
		RuleFor(x => x.ExerciseType.ImageUrl)
			.MaximumLength(255).WithMessage("Exercise type image url must not exceed 255 characters.");
	}
}

/// <summary>
///     Handles the <see cref="CreateExerciseTypeCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
public class CreateExerciseTypeCommandHandler(AppDbContext dbContext)
	: IRequestHandler<CreateExerciseTypeCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateExerciseTypeCommand request, CancellationToken cancellationToken)
	{
		var validationResult = await BusinessValidation(request);
		if (validationResult.IsFailure)
		{
			return validationResult;
		}


		var exerciseType = request.ExerciseType.MapCreateRequestToDbEntity();
		await dbContext.ExerciseTypes.AddAsync(exerciseType, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(exerciseType.Id);
	}

	private async Task<Result<int>> BusinessValidation(CreateExerciseTypeCommand request)
	{
		if (!request.IsAdmin)
		{
			return Result.Failure<int>(ErrorTypes.Unauthorized);
		}

		var existingExerciseType = await dbContext.ExerciseTypes.AnyAsync(x => x.Name == request.ExerciseType.Name);
		if (existingExerciseType)
		{
			return Result.Failure<int>(ErrorTypes.NamingConflict);
		}

		return Result.Success(0);
	}
}

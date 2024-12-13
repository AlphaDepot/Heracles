using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MuscleGroups.Commands;

/// <summary>
///     Represents the request to create a new <see cref="MuscleGroup" />.
/// </summary>
/// <param name="Name">The name of the <see cref="MuscleGroup" />.</param>
public record CreateMuscleGroupRequest(string Name);

/// <summary>
///     Creates a new <see cref="MuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="MuscleGroup">The <see cref="CreateMuscleGroupRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record CreateMuscleGroupCommand(CreateMuscleGroupRequest MuscleGroup, bool IsAdmin = true)
	: IRequest<Result<int>>;

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
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class CreateMuscleGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<CreateMuscleGroupCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateMuscleGroupCommand request, CancellationToken cancellationToken)
	{
		var validationResult = await BusinessValidation(request);
		if (validationResult.IsFailure)
		{
			return validationResult;
		}

		var muscleGroup = request.MuscleGroup.MapCreateRequestToDbEntity();
		await dbContext.MuscleGroups.AddAsync(muscleGroup, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(muscleGroup.Id);
	}

	private async Task<Result<int>> BusinessValidation(CreateMuscleGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return Result.Failure<int>(ErrorTypes.Unauthorized);
		}

		var existingMuscleGroup = await dbContext.MuscleGroups
			.AnyAsync(x => x.Name == request.MuscleGroup.Name);
		if (existingMuscleGroup)
		{
			return Result.Failure<int>(ErrorTypes.NamingConflict);
		}

		return Result.Success(0);
	}
}

using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MuscleFunctions.Commands;

/// <summary>
///     Represents the request to create a new <see cref="MuscleFunction" />.
/// </summary>
/// <param name="Name">The name of the <see cref="MuscleFunction" />.</param>
public record CreateMuscleFunctionRequest(string Name);

/// <summary>
///     Creates a new <see cref="MuscleFunction" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="MuscleFunction">The <see cref="CreateMuscleFunctionRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record CreateMuscleFunctionCommand(CreateMuscleFunctionRequest MuscleFunction, bool IsAdmin = true)
	: IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateMuscleFunctionCommand" />.
/// </summary>
public class CreateMuscleFunctionCommandValidator : AbstractValidator<CreateMuscleFunctionCommand>
{
	public CreateMuscleFunctionCommandValidator()
	{
		RuleFor(x => x.MuscleFunction.Name)
			.NotEmpty().WithMessage("Name is required")
			.Length(3, 50).WithMessage("Name must be between 3 and 50 characters");
	}
}

/// <summary>
///     Handles the <see cref="CreateMuscleFunctionCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class CreateMuscleFunctionCommandHandler(AppDbContext dbContext)
	: IRequestHandler<CreateMuscleFunctionCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateMuscleFunctionCommand request, CancellationToken cancellationToken)
	{
		var validationResult = await BusinessValidation(request);
		if (validationResult.IsFailure)
		{
			return validationResult;
		}

		var muscleFunction = request.MuscleFunction.MapCreateRequestToDbEntity();
		await dbContext.MuscleFunctions.AddAsync(muscleFunction, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(muscleFunction.Id);
	}

	private async Task<Result<int>> BusinessValidation(CreateMuscleFunctionCommand request)
	{
		if (!request.IsAdmin)
		{
			return Result.Failure<int>(ErrorTypes.Unauthorized);
		}

		var existingMuscleFunction = await dbContext.MuscleFunctions
			.AnyAsync(x => x.Name == request.MuscleFunction.Name);
		if (existingMuscleFunction)
		{
			return Result.Failure<int>(ErrorTypes.NamingConflict);
		}

		return Result.Success(0);
	}
}

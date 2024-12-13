using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MuscleFunctions.Commands;

/// <summary>
///     Updates a <see cref="MuscleFunction" />.
/// </summary>
/// <param name="Id"> The Id of the <see cref="MuscleFunction" /> to update.</param>
/// <param name="Name">The new name of the <see cref="MuscleFunction" />.</param>
/// <param name="Concurrency">The concurrency token of the <see cref="MuscleFunction" />.</param>
public record UpdateMuscleFunctionRequest(int Id, string Name, string? Concurrency);

/// <summary>
///     Updates a <see cref="MuscleFunction" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="MuscleFunction">The <see cref="UpdateMuscleFunctionRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateMuscleFunctionCommand(UpdateMuscleFunctionRequest MuscleFunction, bool IsAdmin = true)
	: IRequest<Result<bool>>;

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
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class UpdateMuscleFunctionCommandHandler(AppDbContext dbContext)
	: IRequestHandler<UpdateMuscleFunctionCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateMuscleFunctionCommand request, CancellationToken cancellationToken)
	{
		var (validation, muscleFunction) = await BusinessValidation(request);
		if (validation.IsFailure || muscleFunction == null)
		{
			return validation;
		}


		var updatedMuscleFunction = request.MuscleFunction.MapUpdateRequestToDbEntity(muscleFunction);
		dbContext.Entry(muscleFunction).CurrentValues.SetValues(updatedMuscleFunction);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(
				ErrorTypes.DatabaseErrorWithMessage($"Failed to update Muscle Function {muscleFunction.Id}"));
	}

	private async Task<(Result<bool>, MuscleFunction?)> BusinessValidation(UpdateMuscleFunctionCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var existingMuscleFunction = await dbContext.MuscleFunctions.FindAsync(request.MuscleFunction.Id);
		if (existingMuscleFunction == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		if (existingMuscleFunction.Concurrency != request.MuscleFunction.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}

		var nameAlreadyExists = await dbContext.MuscleFunctions.AnyAsync(x =>
			x.Name == request.MuscleFunction.Name &&
			x.Id != request.MuscleFunction.Id);
		if (nameAlreadyExists)
		{
			return (Result.Failure<bool>(ErrorTypes.NamingConflict), null);
		}

		return (Result.Success(true), existingMuscleFunction);
	}
}

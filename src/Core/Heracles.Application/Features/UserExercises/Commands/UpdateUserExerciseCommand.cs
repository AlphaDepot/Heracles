using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Heracles.Shared.Requests.UserExercises;
using Mediator;

namespace Heracles.Application.Features.UserExercises.Commands;

/// <summary>
///     Updates a <see cref="UserExercise" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="UserExercise">The <see cref="UpdateUserExerciseRequest" /> to update.</param>
public record UpdateUserExerciseCommand(UpdateUserExerciseRequest UserExercise)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateUserExerciseCommand" />.
/// </summary>
public class UpdateUserExerciseCommandValidator : AbstractValidator<UpdateUserExerciseCommand>
{
	public UpdateUserExerciseCommandValidator()
	{
		RuleFor(x => x.UserExercise.Id)
			.NotEmpty().WithMessage("Id is required")
			.GreaterThan(0).WithMessage("Id must be greater than 0");

		RuleFor(x => x.UserExercise.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateUserExerciseCommand" />.
/// </summary>
/// <param name="exerciseRepo"> The <see cref="IUserExercisesRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class UpdateUserExerciseCommandHandler(
	IUserExercisesRepository exerciseRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<UpdateUserExerciseCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(UpdateUserExerciseCommand request, CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		var updated = request.UserExercise.MapUpdateRequestToDbEntity(entity);

		// Apply updated values
		entity.StaticResistance = updated.StaticResistance;
		entity.PercentageResistance = updated.PercentageResistance;
		entity.CurrentWeight = updated.CurrentWeight;
		entity.PersonalRecord = updated.PersonalRecord;
		entity.DurationInSeconds = updated.DurationInSeconds;
		entity.SortOrder = updated.SortOrder;
		entity.Repetitions = updated.Repetitions;
		entity.Sets = updated.Sets;
		entity.Timed = updated.Timed;
		entity.BodyWeight = updated.BodyWeight;

		entity.UpdatedAt = DateTime.UtcNow;
		entity.Concurrency = Guid.NewGuid().ToString();

		await exerciseRepo.SaveChangesAsync(token);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, UserExercise?)> BusinessValidation(
		UpdateUserExerciseCommand request,
		CancellationToken token)
	{
		// check if the user exercise exists
		var entity = await exerciseRepo.GetByIdAsync(request.UserExercise.Id, token);
		if (entity is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		// check if the user is authorized to update the user exercise
		if (currentUser.UserId != entity.UserId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		// validate concurrency
		if (entity.Concurrency != request.UserExercise.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		return (Result.Ok(true), entity);
	}
}

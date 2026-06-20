using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Heracles.Shared.Requests.UserExerciseHistories;
using Mediator;

namespace Heracles.Application.Features.UserExerciseHistories.Commands;

/// <summary>
///     Updates a <see cref="UserExerciseHistory" />.
/// </summary>
/// <param name="UserExerciseHistory"> The <see cref="UpdateUserExerciseHistoryRequest" /> to update.</param>
public record UpdateUserExerciseHistoryCommand(UpdateUserExerciseHistoryRequest UserExerciseHistory)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateUserExerciseHistoryCommand" />.
/// </summary>
public class UpdateUserExerciseHistoryCommandValidator : AbstractValidator<UpdateUserExerciseHistoryCommand>
{
	public UpdateUserExerciseHistoryCommandValidator()
	{
		RuleFor(x => x.UserExerciseHistory.Id)
			.NotEmpty().WithMessage("Id is required")
			.GreaterThan(0).WithMessage("Id must be greater than 0");

		RuleFor(x => x.UserExerciseHistory.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");

		RuleFor(x => x.UserExerciseHistory.UserExerciseId)
			.GreaterThan(0).WithMessage("UserExerciseId is required");

		RuleFor(x => x.UserExerciseHistory.Weight)
			.GreaterThanOrEqualTo(0).WithMessage("Weight is required");

		RuleFor(x => x.UserExerciseHistory.Repetition)
			.GreaterThanOrEqualTo(0).WithMessage("Repetition is required");

		RuleFor(x => x.UserExerciseHistory.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateUserExerciseHistoryCommand" />.
/// </summary>
/// <param name="historyRepo"> The <see cref="IUserExerciseHistoriesRepository" />.</param>
/// <param name="userRepo"> The <see cref="IUsersRepository" />.</param>
/// <param name="userExerciseRepo"> The <see cref="IUserExercisesRepository" />.</param>
/// <param name="currentUser"> The <see cref="ICurrentUserService" />.</param>
public class UpdateUserExerciseHistoryCommandHandler(
	IUserExerciseHistoriesRepository historyRepo,
	IUsersRepository userRepo,
	IUserExercisesRepository userExerciseRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<UpdateUserExerciseHistoryCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(UpdateUserExerciseHistoryCommand request, CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		entity.Weight = request.UserExerciseHistory.Weight;
		entity.Repetition = request.UserExerciseHistory.Repetition;
		entity.UpdatedAt = DateTime.UtcNow;
		entity.Concurrency = Guid.NewGuid().ToString();

		await historyRepo.SaveChangesAsync(token);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, UserExerciseHistory?)> BusinessValidation(
		UpdateUserExerciseHistoryCommand request,
		CancellationToken token)
	{
		// check if the user exists
		var userExists = await userRepo.ExistByUserIdAsync(request.UserExerciseHistory.UserId, token);
		if (!userExists)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithEntityName(nameof(User))), null);
		}

		// check if the user exercise exists
		var exerciseExists = await userExerciseRepo.ExistsByIdAsync(request.UserExerciseHistory.UserExerciseId, token);
		if (!exerciseExists)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithEntityName(nameof(UserExercise))), null);
		}

		// check if the user exercise history exists
		var entity = await historyRepo.GetByIdAsync(request.UserExerciseHistory.Id, token);
		if (entity is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithEntityName(nameof(UserExerciseHistory))), null);
		}

		// check if the user is authorized to update the user exercise history
		if (currentUser.UserId != entity.UserId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		// validate concurrency
		if (entity.Concurrency != request.UserExerciseHistory.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		return (Result.Ok(true), entity);
	}
}

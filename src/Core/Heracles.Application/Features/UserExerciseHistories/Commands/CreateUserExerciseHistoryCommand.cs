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
///     Creates a new <see cref="UserExerciseHistory" />.
/// </summary>
/// <param name="UserExerciseHistory"> The <see cref="CreateUserExerciseHistoryRequest" /> to create.</param>
/// <param name="IsAdmin"> The <see cref="CreateUserExerciseHistoryRequest" /> to create.</param>
public record CreateUserExerciseHistoryCommand(
	CreateUserExerciseHistoryRequest UserExerciseHistory,
	bool IsAdmin = true)
	: IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateUserExerciseHistoryCommand" />.
/// </summary>
public class CreateUserExerciseHistoryValidator : AbstractValidator<CreateUserExerciseHistoryCommand>
{
	public CreateUserExerciseHistoryValidator()
	{
		RuleFor(x => x.UserExerciseHistory.UserExerciseId).GreaterThan(0).WithMessage("UserExerciseId is required");
		RuleFor(x => x.UserExerciseHistory.Weight).GreaterThanOrEqualTo(0).WithMessage("Weight is required");
		RuleFor(x => x.UserExerciseHistory.Repetition).GreaterThanOrEqualTo(0).WithMessage("Repetition is required");
		RuleFor(x => x.UserExerciseHistory.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="CreateUserExerciseHistoryCommand" />.
/// </summary>
/// <param name="historyRepo"> The <see cref="IUserExerciseHistoriesRepository" />.</param>
/// <param name="userRepo"> The <see cref="IUsersRepository" />.</param>
/// <param name="userExerciseRepo"> The <see cref="IUserExercisesRepository" />.</param>
/// <param name="currentUser"> The <see cref="ICurrentUserService" />.</param>
public class CreateUserExerciseHistoryCommandHandler(
	IUserExerciseHistoriesRepository historyRepo,
	IUsersRepository userRepo,
	IUserExercisesRepository userExerciseRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<CreateUserExerciseHistoryCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(CreateUserExerciseHistoryCommand request, CancellationToken token)
	{
		var validation = await BusinessValidation(request, token);
		if (validation.IsFailed)
		{
			return validation;
		}

		var entity = request.UserExerciseHistory.MapCreateRequestToDbEntity();

		await historyRepo.AddAsync(entity, token);
		await historyRepo.SaveChangesAsync(token);

		return Result.Ok(entity.Id);
	}

	private async ValueTask<Result<int>> BusinessValidation(
		CreateUserExerciseHistoryCommand request,
		CancellationToken token)
	{
		// Check if the user exists

		var userExists = await userRepo.ExistByUserIdAsync(request.UserExerciseHistory.UserId, token);
		if (!userExists)
		{
			return Result.Fail<int>(ErrorTypes.NotFoundWithEntityName(nameof(User)));
		}

		// Check if the userid matches the current user
		if (currentUser.UserId != request.UserExerciseHistory.UserId)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		// Check if the user exercise exists
		var exerciseExists = await userExerciseRepo.ExistsByIdAsync(request.UserExerciseHistory.UserExerciseId, token);
		if (!exerciseExists)
		{
			return Result.Fail<int>(ErrorTypes.NotFoundWithEntityName(nameof(UserExercise)));
		}

		return Result.Ok(0);
	}
}

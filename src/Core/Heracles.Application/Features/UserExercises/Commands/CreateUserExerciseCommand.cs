using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Heracles.Shared.Requests.UserExercises;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.UserExercises.Commands;

/// <summary>
///     Creates a new <see cref="UserExercise" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="UserExercise">The <see cref="CreateUserExerciseRequest" /> to create.</param>
public record CreateUserExerciseCommand(CreateUserExerciseRequest UserExercise)
	: IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateUserExerciseCommand" />.
/// </summary>
public class CreateUserExerciseCommandValidator : AbstractValidator<CreateUserExerciseCommand>
{
	public CreateUserExerciseCommandValidator()
	{
		RuleFor(x => x.UserExercise.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");

		RuleFor(x => x.UserExercise.ExerciseTypeId)
			.NotEmpty().WithMessage("ExerciseTypeId is required")
			.GreaterThan(0).WithMessage("ExerciseTypeId must be greater than 0");
	}
}

/// <summary>
///     Handles the <see cref="CreateUserExerciseCommand" />.
/// </summary>
/// <param name="exerciseRepo"> The <see cref="IUserExercisesRepository" />.</param>
/// <param name="userRepo"> The <see cref="IUsersRepository" />.</param>
/// <param name="exerciseTypeRepo"> The <see cref="IExerciseTypesRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class CreateUserExerciseCommandHandler(
	IUserExercisesRepository exerciseRepo,
	IUsersRepository userRepo,
	IExerciseTypesRepository exerciseTypeRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<CreateUserExerciseCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(CreateUserExerciseCommand request, CancellationToken token)
	{
		var validation = await BusinessValidation(request, token);
		if (validation.IsFailed)
		{
			return validation;
		}

		var existingId = await exerciseRepo.GetOwnedByUserAsync(request.UserExercise.ExerciseTypeId, request.UserExercise.UserId, token);

		var entity = request.UserExercise.MapCreateRequestToDbEntity();
		entity.Version = existingId?.Id > 0 ? existingId.Id + 1 : 1;

		await exerciseRepo.AddAsync(entity, token);
		await exerciseRepo.SaveChangesAsync(token);

		return Result.Ok(entity.Id);
	}


	private async ValueTask<Result<int>> BusinessValidation(CreateUserExerciseCommand request, CancellationToken token)
	{
		// check if the user exists
		var userExists = await userRepo.ExistByUserIdAsync(request.UserExercise.UserId, token);
		if (!userExists)
		{
			return Result.Fail<int>(ErrorTypes.NotFoundWithEntityName(nameof(User)));
		}

		// check if the user is the same as the current user
		if (currentUser.UserId != request.UserExercise.UserId)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		// check if the exercise type exists
		var exerciseTypeExists = await exerciseTypeRepo.ExistsByIdAsync(request.UserExercise.ExerciseTypeId, token);
		if (!exerciseTypeExists)
		{
			return Result.Fail<int>(ErrorTypes.NotFoundWithEntityName(nameof(ExerciseType)));
		}

		return Result.Ok(0);
	}
}

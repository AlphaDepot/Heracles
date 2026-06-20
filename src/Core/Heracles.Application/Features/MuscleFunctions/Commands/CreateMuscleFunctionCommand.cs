using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.MuscleFunctions;

namespace Heracles.Application.Features.MuscleFunctions.Commands;

/// <summary>
///     Creates a new <see cref="MuscleFunction" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="MuscleFunction">The <see cref="CreateMuscleFunctionRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record CreateMuscleFunctionCommand(CreateMuscleFunctionRequest MuscleFunction, bool IsAdmin = true)
	: Mediator.IRequest<Result<int>>;

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
/// <param name="repository">The <see cref="IMuscleFunctionsRepository" />.</param>
public class CreateMuscleFunctionCommandHandler(IMuscleFunctionsRepository repository)
	: Mediator.IRequestHandler<CreateMuscleFunctionCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(CreateMuscleFunctionCommand request, CancellationToken token)
	{
		var validation = await BusinessValidation(request, token);
		if (validation.IsFailed)
		{
			return validation;
		}

		var entity = request.MuscleFunction.MapCreateRequestToDbEntity();

		await repository.AddAsync(entity, token);
		await repository.SaveChangesAsync(token);

		return Result.Ok(entity.Id);
	}

	private async ValueTask<Result<int>> BusinessValidation(CreateMuscleFunctionCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		var exists = await repository.ExistsByNameAsync(request.MuscleFunction.Name, token);
		if (exists)
		{
			return Result.Fail<int>(ErrorTypes.NamingConflict);
		}

		return Result.Ok(0);
	}
}

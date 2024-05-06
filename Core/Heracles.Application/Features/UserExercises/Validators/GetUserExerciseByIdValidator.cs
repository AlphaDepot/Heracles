using FluentValidation;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.Users.Interfaces;

namespace Heracles.Application.Features.UserExercises.Validators;

/// <summary>
/// Validates the input for the GetUserExerciseById operation.
/// </summary>
public class GetUserExerciseByIdValidator : AbstractValidator<int>
{
    private readonly IUserExerciseRepository _exerciseRepository;


    public GetUserExerciseByIdValidator(IUserExerciseRepository exerciseRepository, 
        IUserService userService,  string userId)
    {
        _exerciseRepository = exerciseRepository;

        
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Id must be greater than 0")
            .MustAsync(ExerciseExists).WithMessage("Exercise does not exist")
            .MustAsync((id, token) => userService.IsUserAuthorized(id.ToString(), userId))
            .WithMessage("Not authorized to view this exercise");
    }

    /// <summary>
    /// Checks if an exercise with the given id exists.
    /// </summary>
    /// <param name="id">The id of the exercise.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>True if the exercise exists, otherwise false.</returns>
    private Task<bool> ExerciseExists(int id, CancellationToken token)
    {
        return _exerciseRepository.ItExist(id);
    }
}
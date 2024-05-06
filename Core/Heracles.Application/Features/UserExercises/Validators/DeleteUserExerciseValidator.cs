
using FluentValidation;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.Users.Interfaces;

namespace Heracles.Application.Features.UserExercises.Validators;

/// <summary>
/// Validator for deleting a user exercise.
/// </summary>
public class DeleteUserExerciseValidator : AbstractValidator<int>
{
    private readonly IUserExerciseRepository _exerciseRepository;


    public DeleteUserExerciseValidator(IUserExerciseRepository exerciseRepository,
        IUserService userService, string userId)
    {
        _exerciseRepository = exerciseRepository;

        
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Id must be greater than 0")
            .MustAsync(ExerciseExists).WithMessage("Exercise does not exist") 
            .MustAsync((id, token) => userService.IsUserAuthorized(id.ToString(), userId))
            .WithMessage("Not authorized to delete this exercise");
    }


    /// <summary>
    /// Checks if a user exercise exists.
    /// </summary>
    /// <param name="id">The ID of the user exercise.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating if the user exercise exists.</returns>
    private Task<bool> ExerciseExists(int id, CancellationToken token)
    {
        return _exerciseRepository.ItExist(id);
    }
}
using FluentValidation;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.Users.Interfaces;

namespace Heracles.Application.Features.UserExercisesHistories.Validators;

/// <summary>
/// Validator class for creating a user exercise history.
/// </summary>
public class CreateUserExerciseHistoryValidator : AbstractValidator<UserExerciseHistory>
{
    private readonly IUserExerciseRepository _userExerciseRepository;
    
    public CreateUserExerciseHistoryValidator(IUserExerciseRepository userExerciseRepository, 
        IUserService userService,  string currentUserId)
    {
        _userExerciseRepository = userExerciseRepository;
        

        RuleFor(x => x.UserExerciseId)
            .NotEmpty().WithMessage("UserExerciseId is required")
            .MustAsync(UserExerciseExists).WithMessage("UserExerciseId does not exist");
        
        RuleFor(x => x.Weight)
            .NotEmpty().WithMessage("Weight is required")
            .GreaterThan(0).WithMessage("Weight must be greater than 0");
        
        RuleFor(x => x.Repetition)
            .NotEmpty().WithMessage("Repetition is required")
            .GreaterThan(0).WithMessage("Repetition must be greater than 0");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MustAsync((user_id, token) 
                => userService.IsUserAuthorized(user_id, currentUserId))
            .WithMessage("You are not authorized to create a user exercise history for another user")
            .MustAsync((userId, token) => userService.DoesUserExist(userId))
            .WithMessage(x => $"User with id {x.UserId} does not exist");
    }

    /// <summary>
    /// Checks if a user exercise exists.
    /// </summary>
    /// <param name="exerciseId">The ID of the user exercise.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the user exercise exists or not.</returns>
    private Task<bool> UserExerciseExists(int exerciseId, CancellationToken token)
    {
        return _userExerciseRepository.ItExist(exerciseId);
    }
    
 
    
}
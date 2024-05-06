using FluentValidation;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.Users.Interfaces;

namespace Heracles.Application.Features.UserExercises.Validators;

/// <summary>
/// Validator for creating user exercises.
/// </summary>
public class CreateUserExerciseValidator : AbstractValidator<CreateUserExerciseDto>
{
    private readonly IExerciseTypeRepository _exerciseRepository;



    public CreateUserExerciseValidator(IExerciseTypeRepository exerciseRepository,
        IUserService userService,  string currentUserId)
    {
        _exerciseRepository = exerciseRepository;
    
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MustAsync((user_id, token) 
                => userService.IsUserAuthorized(user_id, currentUserId))
            .WithMessage("You are not authorized to create a user exercise for another user")
            .MustAsync((userId, token) => userService.DoesUserExist(userId))
            .WithMessage(x => $"User with id {x.UserId} does not exist");
            
        
        RuleFor(x => x.ExerciseId)
            .NotEmpty().WithMessage("ExerciseTypeId is required")
            .MustAsync(ExerciseExists).WithMessage("Exercise does not exist");
    }
    

    /// <summary>
    ///  Check if the exercise exists
    /// </summary>
    /// <param name="exerciseId"> The exercise id to be checked </param>
    /// <param name="token"> The cancellation token </param>
    /// <returns> True if the exercise exists, false otherwise </returns>
    private Task<bool> ExerciseExists(int exerciseId, CancellationToken token)
    {
        return _exerciseRepository.ItExist(exerciseId);
    }
    
    
}
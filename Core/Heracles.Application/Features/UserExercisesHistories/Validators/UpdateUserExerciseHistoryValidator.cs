using FluentValidation;
using Heracles.Domain.UserExerciseHistories.DTOs;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.Users.Interfaces;

namespace Heracles.Application.Features.UserExercisesHistories.Validators;

/// <summary>
///   Validator for updating a user exercise history
/// </summary>
public class UpdateUserExerciseHistoryValidator  : AbstractValidator<UpdateUserExerciseHistoryDto>
{
    private readonly IUserExerciseHistoryRepository _userExerciseHistoryRepository;

    public UpdateUserExerciseHistoryValidator(IUserExerciseHistoryRepository userExerciseHistoryRepository, 
        IUserService userService, string userId)
    {
        _userExerciseHistoryRepository = userExerciseHistoryRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(HistoryExist).WithMessage("Id does not exist");
        
        RuleFor(x => x.Weight)
            .NotEmpty().WithMessage("Weight is required")
            .GreaterThan(0).WithMessage("Weight must be greater than 0");
        
        RuleFor(x => x.Repetition)
            .NotEmpty().WithMessage("Repetition is required")
            .GreaterThan(0).WithMessage("Repetition must be greater than 0");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MustAsync((user_id, token)
                => userService.IsUserAuthorized(user_id, userId))
            .WithMessage("You are not authorized to update a user exercise history for another user")
            .MustAsync((userId, token) => userService.DoesUserExist(userId))
            .WithMessage(x => $"User with id {x.UserId} does not exist");



    }

    /// <summary>
    /// Check if a user exercise history exists
    /// </summary>
    /// <param name="id">The id of the user exercise history</param>
    /// <param name="token">The cancellation token</param>
    /// <returns>Returns a task that represents the asynchronous operation. The task result contains true if the user exercise history exists; otherwise, false.</returns>
    private Task<bool> HistoryExist(int id, CancellationToken token)
    {
        return _userExerciseHistoryRepository.ItExist(id);
    }
    
    
}
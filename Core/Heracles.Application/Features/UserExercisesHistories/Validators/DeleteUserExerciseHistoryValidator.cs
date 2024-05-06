using FluentValidation;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.Users.Interfaces;

namespace Heracles.Application.Features.UserExercisesHistories.Validators;

/// <summary>
/// Validates the deletion of a user exercise history.
/// </summary>
public class DeleteUserExerciseHistoryValidator : AbstractValidator<int>
{
    private readonly IUserExerciseHistoryRepository _userExerciseHistoryRepository;
    
    public DeleteUserExerciseHistoryValidator(IUserExerciseHistoryRepository userExerciseHistoryRepository, 
        IUserService userService, string userId)
    {
        
        _userExerciseHistoryRepository = userExerciseHistoryRepository;


        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Id must be greater than 0")
            .MustAsync(HistoryExist).WithMessage("Id does not exist")
            .MustAsync((id, token) => userService.IsUserAuthorized(id.ToString(), userId))
            .WithMessage("You are not authorized to delete a user exercise history for another user");

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
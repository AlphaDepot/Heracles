using FluentValidation;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.Users.Interfaces;

namespace Heracles.Application.Features.UserExercisesHistories.Validators;

/// <summary>
/// Validates the input when retrieving a user exercise history by its ID.
/// </summary>
public class GetUserExerciseHistoryByIdValidator : AbstractValidator<int>
{
    private readonly IUserExerciseHistoryRepository _userExerciseHistoryRepository;
    
    public GetUserExerciseHistoryByIdValidator(IUserExerciseHistoryRepository userExerciseHistoryRepository,
        IUserService userService,  string userId)
    {
        _userExerciseHistoryRepository = userExerciseHistoryRepository;

        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Id must be greater than 0")
            .MustAsync(HistoryExist).WithMessage("Id does not exist")
            .MustAsync((id, token) => userService.IsUserAuthorized(id.ToString(), userId))
            .WithMessage("You are not authorized to view a user exercise history for another user");
    }

    /// <summary>
    /// Checks if a user exercise history with the given ID exists in the repository.
    /// </summary>
    /// <param name="id">The ID of the user exercise history to check.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>True if the user exercise history exists, false otherwise.</returns>
    private Task<bool> HistoryExist(int id, CancellationToken token)
    {
        return _userExerciseHistoryRepository.ItExist(id);
    }
    
}
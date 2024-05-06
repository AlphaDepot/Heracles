using FluentValidation;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;

namespace Heracles.Application.Features.Users.Validators;

/// <summary>
/// Validates the data for updating a user.
/// </summary>
public class UpdateUserValidator  : AbstractValidator<User>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(ItExists).WithMessage("User does not exist");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MinimumLength(2).WithMessage("UserId must be at least 2 characters")
            .MaximumLength(255).WithMessage("UserId cannot be longer than 255 characters")
            .MustAsync((x, y, z) => ItsUnique(x.UserId, x.Id, z)).WithMessage("UserId already exists");
    }

    /// <summary>
    /// Checks if the given userId is unique for updating a user.
    /// </summary>
    /// <param name="userId">The userId to check.</param>
    /// <param name="id">The id of the user being updated.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>True if the userId is unique, false otherwise.</returns>
    private Task<bool> ItsUnique(string userId, int id, CancellationToken token)
    {
        // User exists with the same userId and Id Combination
        return _userRepository.UserIdWithIdExistsAsync(userId, id);
    }

    /// <summary>
    /// Checks if the user with the given id exists in the repository.
    /// </summary>
    /// <param name="id">The id of the user to check.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>True if the user exists, false otherwise.</returns>
    private Task<bool> ItExists(int id, CancellationToken token)
    {
        return _userRepository.ItExist(id);
    }
}
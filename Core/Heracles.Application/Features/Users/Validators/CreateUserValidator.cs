using FluentValidation;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;

namespace Heracles.Application.Features.Users.Validators;

/// <summary>
/// Validator class for creating a new user.
/// </summary>
public class CreateUserValidator : AbstractValidator<User>
{
    private readonly IUserRepository _userRepository;

    public CreateUserValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MinimumLength(2).WithMessage("UserId must be at least 2 characters")
            .MaximumLength(255).WithMessage("UserId cannot be longer than 255 characters")
            .MustAsync(ItsUnique).WithMessage(x => $"UserId {x.UserId} already exists");
    }

    /// <summary>
    /// Checks if a given user ID is unique in the user repository.
    /// </summary>
    /// <param name="userId">The user ID to check for uniqueness.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the user ID is unique.</returns>
    private Task<bool> ItsUnique(string userId, CancellationToken token)
    {
        var user = _userRepository.UserIdExistsAsync(userId);

        return Task.FromResult(!user.Result);
    }
}
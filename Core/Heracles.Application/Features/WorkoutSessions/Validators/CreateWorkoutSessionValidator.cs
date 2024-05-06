using FluentValidation;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.WorkoutSessions.Interfaces;
using Heracles.Domain.WorkoutSessions.Models;

namespace Heracles.Application.Features.WorkoutSessions.Validators;

public class CreateWorkoutSessionValidator : AbstractValidator<WorkoutSession>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public CreateWorkoutSessionValidator(IWorkoutSessionRepository sessionRepository,
        IUserService userService,  string currentUserId)
    {
        _sessionRepository = sessionRepository;
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MustAsync((userId, token) => userService.DoesUserExist(userId))
            .WithMessage(x => $"User with id {x.UserId} does not exist")
            .MustAsync((user_id, token) 
                => userService.IsUserAuthorized(user_id, currentUserId))
            .WithMessage("You are not authorized to create a workout session for another user");
        
        RuleFor(x => x.DayOfWeek)
            .NotEmpty().WithMessage("DayOfWeek is required")
            .IsInEnum().WithMessage("Invalid DayOfWeek");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name is too long")
            .MinimumLength(3).WithMessage("Name is too short")
            .MustAsync(NameExist).WithMessage("Name already exists");

        

    }
    
    private Task<bool> NameExist(string name, CancellationToken token)
    {
        return _sessionRepository.IsUnique(name);
    }
    
 
}
using FluentValidation;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.WorkoutSessions.DTOs;
using Heracles.Domain.WorkoutSessions.Interfaces;

namespace Heracles.Application.Features.WorkoutSessions.Validators;

public class WorkoutSessionExerciseValidator : AbstractValidator<WorkoutSessionExerciseDto>
{
    
    private readonly IWorkoutSessionRepository _sessionRepository;
    private readonly IUserExerciseRepository _userExerciseRepository;

    private readonly string _currentUserId;

    public WorkoutSessionExerciseValidator(
        IWorkoutSessionRepository sessionRepository, 
        IUserExerciseRepository userExerciseRepository,
        IUserService userService, string currentUserId)
    {
        _sessionRepository = sessionRepository;
        _userExerciseRepository = userExerciseRepository;

        _currentUserId = currentUserId;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MustAsync((user_id, token) 
                => userService.IsUserAuthorized(user_id, _currentUserId))
            .WithMessage("You are not authorized to create a workout session exercise for another user")
            .MustAsync((userId, token) => userService.DoesUserExist(userId))
            .WithMessage(x => $"User with id {x.UserId} does not exist");

        RuleFor(x => x.WorkoutSessionId)
            .NotEmpty().WithMessage("WorkoutSessionId is required")
            .MustAsync(SessionExists).WithMessage("WorkoutSession does not exist");
        
        
        RuleFor(x => x.UserExerciseId)
            .NotEmpty().WithMessage("UserExerciseId is required")
            .MustAsync(UserExerciseExists).WithMessage("UserExercise does not exist");
        
        
    }
    
    private Task<bool> SessionExists(int id, CancellationToken token)
    {
        return _sessionRepository.ItExist(id);
    }
    
    private Task<bool> UserExerciseExists(int id, CancellationToken token)
    {
        return _userExerciseRepository.ItExist(id);
    }
    
    
    
}
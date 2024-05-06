using FluentValidation;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.WorkoutSessions.Interfaces;

namespace Heracles.Application.Features.WorkoutSessions.Validators;

public class GetWorkoutSessionByIdValidator : AbstractValidator<int>
{
    private readonly IWorkoutSessionRepository _sessionRepository;


    public GetWorkoutSessionByIdValidator(IWorkoutSessionRepository sessionRepository, IUserService userService,
        string currentUserId)
    {
        _sessionRepository = sessionRepository;
        
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Id must be greater than 0")
            .MustAsync(SessionExists).WithMessage("Id does not exist")
            .MustAsync((id, token) => userService.IsUserAuthorized(id.ToString(), currentUserId))
            .WithMessage("You are not authorized to view  a workout session for another user");
    }

    private Task<bool> SessionExists(int id, CancellationToken token)
    {
        return _sessionRepository.ItExist(id);
    }
    
 
}
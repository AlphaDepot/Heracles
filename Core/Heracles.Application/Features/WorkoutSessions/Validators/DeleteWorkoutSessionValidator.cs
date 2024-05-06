using FluentValidation;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.WorkoutSessions.Interfaces;

namespace Heracles.Application.Features.WorkoutSessions.Validators;

public class DeleteWorkoutSessionValidator : AbstractValidator<int>
{
    private readonly IWorkoutSessionRepository _sessionRepository;


    public DeleteWorkoutSessionValidator(IWorkoutSessionRepository sessionRepository, IUserService userService, string userId)
    {
        _sessionRepository = sessionRepository;

        
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Id must be greater than 0")
            .MustAsync(SessionExists).WithMessage("Session does not exist") 
            .MustAsync((id, token) => userService.IsUserAuthorized(id.ToString(), userId))
            .WithMessage("Not authorized to delete this session");
        
    }
    
    
    private Task<bool> SessionExists(int id, CancellationToken token)
    {
        return _sessionRepository.ItExist(id);
    }
    

}
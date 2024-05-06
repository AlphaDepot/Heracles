using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.WorkoutSessions.Models;

namespace Heracles.Domain.WorkoutSessions.Interfaces;

public interface IWorkoutSessionRepository : IGenericRepository<WorkoutSession>
{
    Task<bool> IsUnique(string name);
    
}
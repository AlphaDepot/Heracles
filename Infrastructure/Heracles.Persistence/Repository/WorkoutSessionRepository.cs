
using Heracles.Domain.WorkoutSessions.Interfaces;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class WorkoutSessionRepository : GenericRepository<WorkoutSession>, IWorkoutSessionRepository
{
    public WorkoutSessionRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// Checks if the given name is unique among existing workout sessions.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if the name is unique, false otherwise.</returns>
    public async Task<bool> IsUnique(string name)
    {
        return !await _dbContext.WorkoutSessions.AnyAsync(x => x.Name.ToLower() == name.ToLower());
    }
    
}
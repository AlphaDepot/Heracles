
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
    
    public async Task<bool> IsUnique(string name)
    {
        return !await DbContext.WorkoutSessions.AnyAsync(x => x.Name.ToLower() == name.ToLower());
    }
    
}
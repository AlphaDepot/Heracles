using Heracles.Domain.MuscleGroups;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.MuscleGroups.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class MuscleGroupRepository : GenericRepository<MuscleGroup>, IMuscleGroupRepository
{
    public MuscleGroupRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    ///   Check if the name is unique
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<bool> IsNameUnique(string name)
    {
        return !await DbContext.MuscleGroups
            .AnyAsync(e => e.Name.ToLower() == name.ToLower());
    }
}
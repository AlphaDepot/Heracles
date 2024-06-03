using Heracles.Domain.MuscleFunctions;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleFunctions.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class MuscleFunctionRepository : GenericRepository<MuscleFunction>, IMuscleFunctionRepository
{
    public MuscleFunctionRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<bool> IsNameUnique(string name)
    {
        return !await DbContext.MuscleFunctions.AnyAsync(x => x.Name.ToLower() == name.ToLower());    }
}
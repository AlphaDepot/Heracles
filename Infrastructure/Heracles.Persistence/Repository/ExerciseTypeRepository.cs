using Heracles.Domain.ExercisesType;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.ExercisesType.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class ExerciseTypeRepository: GenericRepository<ExerciseType>, IExerciseTypeRepository
{
    public ExerciseTypeRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<bool> IsNameUnique(string name)
    {
        var exerciseType = await DbContext.ExerciseTypes.Where(et => et.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        return exerciseType == null;
    }
}
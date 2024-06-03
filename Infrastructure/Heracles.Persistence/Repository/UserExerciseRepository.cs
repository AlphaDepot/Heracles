using Heracles.Domain.UserExercises;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.UserExercises.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class UserExerciseRepository : GenericRepository<UserExercise>, IUserExerciseRepository
{
    public UserExerciseRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<UserExercise?> GetUserExerciseByExerciseTypeIdAsync(int id)
    {
        return await DbContext.UserExercises.FirstOrDefaultAsync(x => x.ExerciseTypeId == id);
    }


}
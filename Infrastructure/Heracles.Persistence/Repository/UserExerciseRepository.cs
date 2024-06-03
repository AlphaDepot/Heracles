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

    /// <summary>
    /// Gets a user exercise by exercise type ID asynchronously.
    /// </summary>
    /// <param name="id">The exercise type ID.</param>
    /// <returns>A task that represents the asynchronous operation and contains the user exercise with the specified exercise type ID, or null if not found.</returns>
    public async Task<UserExercise?> GetUserExerciseByExerciseTypeIdAsync(int id)
    {
        return await DbContext.UserExercises.FirstOrDefaultAsync(x => x.ExerciseTypeId == id);
    }


}
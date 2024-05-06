using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.UserExercises.Models;

namespace Heracles.Domain.UserExercises.Interfaces;

public interface IUserExerciseRepository : IGenericRepository<UserExercise>
{
    
    Task<UserExercise?> GetUserExerciseByExerciseTypeIdAsync(int id);

    
}
using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.ExercisesType.Models;

namespace Heracles.Domain.ExercisesType.Interfaces;

public interface IExerciseTypeRepository : IGenericRepository<ExerciseType>
{
    Task<bool> IsNameUnique(string name);
}
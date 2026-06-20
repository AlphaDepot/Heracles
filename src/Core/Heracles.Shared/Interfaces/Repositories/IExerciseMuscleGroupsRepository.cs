using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces.Repositories.Base;

namespace Heracles.Shared.Interfaces.Repositories;

public interface IExerciseMuscleGroupsRepository : IRepository<ExerciseMuscleGroup>
{
	Task<bool> CombinationExistsAsync(int exerciseTypeId, int muscleId, int functionId, CancellationToken token = default);
}

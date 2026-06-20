using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces.Repositories.Base;

namespace Heracles.Shared.Interfaces.Repositories;

public interface IWorkoutSessionRepository : IRepository<WorkoutSession>
{
	Task<bool> NameExistsForUserAsync(string userId, string name, CancellationToken token = default);
}

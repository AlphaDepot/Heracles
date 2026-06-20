using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces.Repositories.Base;

namespace Heracles.Shared.Interfaces.Repositories;

public interface IUserExercisesRepository : IRepository<UserExercise>
{
	Task<UserExercise?> GetOwnedByUserAsync(int id, string userId, CancellationToken token = default);
}

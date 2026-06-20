using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces.Repositories.Base;

namespace Heracles.Shared.Interfaces.Repositories;

public interface IUsersRepository : IRepository<User>
{
	Task<User?> GetByUserIdAsync(string userId, CancellationToken token = default);
	Task<bool> ExistByUserIdAsync(string userId, CancellationToken token = default);
}

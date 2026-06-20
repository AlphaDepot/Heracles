using Heracles.Domain.Interfaces;

namespace Heracles.Shared.Interfaces.Repositories.Base;

public interface INamedRepository<T> : IRepository<T>
	where T : class, IEntity, IHasName
{
	Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
	Task<bool> NameInUseAsync(string name, int id, CancellationToken token = default);
}

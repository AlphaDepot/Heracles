using Heracles.Domain.Interfaces;

namespace Heracles.Shared.Interfaces.Repositories.Base;

public interface IRepository<T> where T : class, IEntity
{
	IQueryable<T> Query();
	IQueryable<T> QueryTracking();
	Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default);
	Task AddAsync(T entity, CancellationToken cancellationToken = default);
	Task RemoveAsync(T entity, CancellationToken cancellationToken = default);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

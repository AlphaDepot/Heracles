using Heracles.Domain.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories.Base;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
	protected readonly AppDbContext _db;

	public Repository(AppDbContext db)
	{
		_db = db;
	}

	public IQueryable<T> Query()
	{
		return _db.Set<T>().AsNoTracking();
	}

	public IQueryable<T> QueryTracking()
		=> _db.Set<T>();

	public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _db.Set<T>().FindAsync(id, cancellationToken);
	}

	public async Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default)
	{
		return await _db.Set<T>().AsNoTracking().AnyAsync(e => e.Id == id, ct);
	}

	public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
	{
		await _db.Set<T>().AddAsync(entity, cancellationToken);
	}

	public Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
	{
		_db.Set<T>().Remove(entity);
		return Task.CompletedTask;
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return _db.SaveChangesAsync(cancellationToken);
	}
}

using Heracles.Domain.Interfaces;
using Heracles.Shared.Interfaces.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories.Base;

public class NamedRepository<T> : Repository<T>, INamedRepository<T>
	where T : class, IEntity, IHasName
{
	public NamedRepository(AppDbContext db) : base(db) {}

	public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
	{
		return _db.Set<T>().AsNoTracking().AnyAsync(x => x.Name == name, ct);
	}

	public Task<bool> NameInUseAsync(string name, int id, CancellationToken token = default)
	{
		return _db.Set<T>().AsNoTracking().AnyAsync(x => x.Name == name && x.Id != id, token);
	}
}

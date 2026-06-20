using Heracles.Domain.Interfaces;
using Heracles.Shared.Interfaces.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories.Base;

public class TypeRepository<T> : Repository<T>, ITypeRepository<T>
	where T : class, IEntity, IHasType
{
	public TypeRepository(AppDbContext db) : base(db) {}

	public Task<bool> ExistByType(string type, CancellationToken ct = default)
	{
		return _db.Set<T>().AnyAsync(x => x.Type == type, ct);
	}
}

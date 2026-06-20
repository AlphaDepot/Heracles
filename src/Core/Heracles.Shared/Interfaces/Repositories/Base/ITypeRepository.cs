using Heracles.Domain.Interfaces;

namespace Heracles.Shared.Interfaces.Repositories.Base;

public interface ITypeRepository<T> : IRepository<T>
	where T : class, IEntity, IHasType
{
	Task<bool> ExistByType(string name, CancellationToken ct = default);
}

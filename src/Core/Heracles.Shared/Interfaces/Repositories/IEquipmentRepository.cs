using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces.Repositories.Base;

namespace Heracles.Shared.Interfaces.Repositories;

public interface IEquipmentRepository : ITypeRepository<Equipment>
{
	Task<bool> NameInUseAsync(string type, int id, CancellationToken token = default);
}

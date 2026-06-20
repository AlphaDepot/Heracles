using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories;

public class EquipmentRepository
	: TypeRepository<Equipment>, IEquipmentRepository
{
	public EquipmentRepository(AppDbContext db) : base(db)
	{
	}

	public Task<bool> NameInUseAsync(string type, int id, CancellationToken token = default)
		=> _db.Equipments.AnyAsync(e => e.Type == type && e.Id != id, token);
}

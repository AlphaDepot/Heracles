using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories;

public class EquipmentGroupRepository
	: NamedRepository<EquipmentGroup>, IEquipmentGroupRepository
{
	public EquipmentGroupRepository(AppDbContext db) : base(db)
	{
	}

	public override async Task<EquipmentGroup?> GetByIdAsync(int id, CancellationToken token = default)
	{
		return await _db.EquipmentGroups
			.Include(g => g.Equipments)
			.FirstOrDefaultAsync(g => g.Id == id, token);
	}

}

using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Persistence.Repositories;

public class MuscleGroupRepository
	: NamedRepository<MuscleGroup>, IMuscleGroupsRepository
{
	public MuscleGroupRepository(AppDbContext db) : base(db)
	{
	}
}

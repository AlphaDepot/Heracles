using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Persistence.Repositories;

public class MuscleFunctionRepository
	: NamedRepository<MuscleFunction>, IMuscleFunctionsRepository
{
	public MuscleFunctionRepository(AppDbContext db) : base(db)
	{
	}
}

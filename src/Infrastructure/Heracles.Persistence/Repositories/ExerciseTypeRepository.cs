using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories;

public class ExerciseTypeRepository
	: NamedRepository<ExerciseType>, IExerciseTypesRepository
{
	public ExerciseTypeRepository(AppDbContext db) : base(db)
	{
	}

	public override async Task<ExerciseType?> GetByIdAsync(int id, CancellationToken token = default)
	{
		return await _db.ExerciseTypes
			.Include(t => t.MuscleGroups)
			.FirstOrDefaultAsync(t => t.Id == id, token);
	}


}

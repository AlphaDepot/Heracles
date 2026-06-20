using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories;

public class ExerciseMuscleGroupRepository
	: Repository<ExerciseMuscleGroup>, IExerciseMuscleGroupsRepository
{
	public ExerciseMuscleGroupRepository(AppDbContext db) : base(db)
	{
	}

	public override async Task<ExerciseMuscleGroup?> GetByIdAsync(int id, CancellationToken token = default)
	{
		return await _db.ExerciseMuscleGroups
			.Include(x => x.Muscle)
			.Include(x => x.Function)
			.FirstOrDefaultAsync(x => x.Id == id, token);
	}

	public Task<bool> CombinationExistsAsync(
		int exerciseTypeId,
		int muscleId,
		int functionId,
		CancellationToken token = default)
	{
		return _db.ExerciseMuscleGroups
			.AsNoTracking()
			.AnyAsync(x =>
					x.ExerciseTypeId == exerciseTypeId &&
					x.MuscleId == muscleId &&
					x.FunctionId == functionId,
				token);
	}
}

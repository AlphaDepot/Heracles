using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories;

public class UserExerciseRepository
	: Repository<UserExercise>, IUserExercisesRepository
{
	public UserExerciseRepository(AppDbContext db) : base(db)
	{
	}

	public override async Task<UserExercise?> GetByIdAsync(int id, CancellationToken token = default)
	{
		return await _db.UserExercises
			.Include(x => x.ExerciseType)
			.ThenInclude(et => et.MuscleGroups)
			.Include(x => x.EquipmentGroup)
			.FirstOrDefaultAsync(x => x.Id == id, token);
	}

	public Task<UserExercise?> GetOwnedByUserAsync(int id, string userId, CancellationToken token = default)
	 => _db.UserExercises
		.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, token);
}

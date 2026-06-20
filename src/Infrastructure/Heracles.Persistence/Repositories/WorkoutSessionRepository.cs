using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories;

public class WorkoutSessionRepository
	: Repository<WorkoutSession>, IWorkoutSessionRepository
{
	public WorkoutSessionRepository(AppDbContext db) : base(db)
	{
	}

	public override async Task<WorkoutSession?> GetByIdAsync(int id, CancellationToken token = default)
	{
		return await _db.WorkoutSessions
			.Include(x => x.UserExercises)
			.FirstOrDefaultAsync(x => x.Id == id, token);
	}

	public Task<bool> NameExistsForUserAsync(string userId, string name, CancellationToken token = default)
	{
		return _db.WorkoutSessions
			.AsNoTracking()
			.AnyAsync(x => x.UserId == userId && x.Name == name, token);
	}
}

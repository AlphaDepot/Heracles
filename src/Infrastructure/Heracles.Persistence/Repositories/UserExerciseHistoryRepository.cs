using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Persistence.Repositories;

public class UserExerciseHistoryRepository
	: Repository<UserExerciseHistory>, IUserExerciseHistoriesRepository
{
	public UserExerciseHistoryRepository(AppDbContext db) : base(db)
	{
	}
}

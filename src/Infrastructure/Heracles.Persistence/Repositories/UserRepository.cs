using Heracles.Domain.Entities;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repositories;

public class UserRepository
	: Repository<User>, IUsersRepository
{
	public UserRepository(AppDbContext db) : base(db)
	{
	}

	public async Task<User?> GetByUserIdAsync(string userId, CancellationToken token = default)
		=> await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId, token);

	public Task<bool> ExistByUserIdAsync(string userId, CancellationToken token = default)
		=> _db.Users.AnyAsync(x => x.UserId == userId, token);
}

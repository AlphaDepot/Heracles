using Heracles.Domain.Entities;

namespace Heracles.Persistence.SeedData;

public abstract class UsersDataLoader
{
	public static void Initialize(AppDbContext context)
	{
		SeedUsers(context);
	}

	private static void SeedUsers(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.Users.Any())
		{
			return;
		}

		context.Users.AddRange(Users());
		context.SaveChanges();
	}

	public static List<User> Users()
	{
		return
		[
			new User
			{
				UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f22",
				Email = "admin.test.user@test.com",
				IsAdmin = true
			},
			new User
			{
				UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f25",
				Email = "test.user@test.com",
				IsAdmin = false
			}
		];
	}
}

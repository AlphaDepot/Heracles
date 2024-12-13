using Application.Features.Users;

namespace Application.Infrastructure.Data.SeedData;

public abstract class UsersDataLoader
{


	public static void Initialize(AppDbContext context)
	{
		// !!! ORDER OF SEED DATA IS IMPORTANT!!! #1#

		// TestData Users first since it is the base data
		SeedUsers(context);
	}

	/// <summary>
	///   TestData users into the database.
	/// </summary>
	/// <param name="context"> The database context to seed the users into.</param>
	private static void SeedUsers(AppDbContext context)
	{
		// ensure the database is created
		context.Database.EnsureCreated();

		// check if the database is already seeded
		if (context.Users.Any())
		{
			return;
		}

		// TestData data
		var users = Users();

		// Add the seed data to the database
		context.Users.AddRange(users);

		// Save the changes
		context.SaveChanges();
	}


	/// <summary>
	///     Dummy data for User
	/// </summary>
	/// <returns></returns>
	public static List<User> Users()
	{
		return
		[
			// admin user
			// admin user
			new User
			{
				UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f22",
				Email = "admin.test.user@test.com",
				IsAdmin = true
			},

			// non admin user
			new User
			{
				UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f25",
				Email = "test.user@test.com",
				IsAdmin = false
			},
			new User
			{
				UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f26",
				Email = "test.user@test.com",
				IsAdmin = false
			}
		];
	}
}

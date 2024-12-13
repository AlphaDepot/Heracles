using Application.Features.UserExerciseHistories;
using Application.Features.UserExercises;
using Application.Features.WorkoutSessions;

namespace Application.Infrastructure.Data.SeedData;

public abstract class UserExerciseDataLoader
{
	public static void Initialize(AppDbContext context)
	{
		// !!! ORDER OF SEED DATA IS IMPORTANT!!! #1#

		// TestData UserExercises second since it depends on Users
		SeedUserExercises(context);

		// TestData WorkoutSessions third since it depends on UserExercises
		SeedWorkoutSessions(context);

		// TestData UserExerciseHistories fourth since it depends on UserExercises
		SeedUserExerciseHistory(context);
	}


	/// <summary>
	///     TestData user exercises into the database.
	/// </summary>
	/// <param name="context">The database context to seed the user exercises into.</param>
	private static void SeedUserExercises(AppDbContext context)
	{
		// ensure the database is created
		context.Database.EnsureCreated();

		// check if the database is already seeded
		if (context.UserExercises.Any())
		{
			return;
		}

		// TestData data
		var userExercises = UserExercises(context);

		// Add the seed data to the database
		context.UserExercises.AddRange(userExercises);

		// Save the changes
		context.SaveChanges();
	}

	/// <summary>
	///     TestData workout sessions into the database.
	/// </summary>
	/// <param name="context">The database context to seed the workout sessions into.</param>
	private static void SeedWorkoutSessions(AppDbContext context)
	{
		// ensure the database is created
		context.Database.EnsureCreated();

		// check if the database is already seeded
		if (context.WorkoutSessions.Any())
		{
			return;
		}

		// TestData data
		var workoutSessions = WorkoutSessions(context);

		// Add the seed data to the database
		context.WorkoutSessions.AddRange(workoutSessions);

		// Save the changes
		context.SaveChanges();
	}


	/// <summary>
	///     TestData user exercise history data into the database.
	/// </summary>
	/// <param name="context">The database context to seed the data into.</param>
	private static void SeedUserExerciseHistory(AppDbContext context)
	{
		// ensure the database is created
		context.Database.EnsureCreated();

		// check if the database is already seeded
		if (context.UserExerciseHistories.Any())
		{
			return;
		}

		// TestData data
		var userExerciseHistories = UserExerciseHistories(context);

		// Add the seed data to the database
		context.UserExerciseHistories.AddRange(userExerciseHistories);

		// Save the changes
		context.SaveChanges();
	}


	/// <summary>
	///     TestData data for UserExercise
	///     It must be the second seed data to be inserted from this category
	///     It depends on ExerciseSeedData.ExerciseTypesController and UserSeedData.Users
	/// </summary>
	/// <returns> List of UserExercise</returns>
	public static List<UserExercise> UserExercises(AppDbContext context)
	{
		var users = UsersDataLoader.Users().ToList();
		var exerciseTypes = context.ExerciseTypes.ToList();

		if (users.Count == 0)
		{
			throw new Exception("User  data is required to seed UserExercise data");
		}

		if (exerciseTypes.Count == 0)
		{
			throw new Exception("ExerciseType data is required to seed UserExercise data");
		}


		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		return
		[
			// User exercises for admin user
			new UserExercise
			{
				ExerciseTypeId = 1, Version = 1, CreatedAt = date, UpdatedAt = date,
				UserId = users.First().UserId, ExerciseType = exerciseTypes.First()
			},
			new UserExercise
			{
				ExerciseTypeId = 2, Version = 1, CreatedAt = date, UpdatedAt = date,
				UserId = users.First().UserId, ExerciseType = exerciseTypes[1]
			},
			new UserExercise
			{
				ExerciseTypeId = 3, Version = 1, CreatedAt = date, UpdatedAt = date,
				UserId = users.First().UserId, ExerciseType = exerciseTypes[2]
			},
			new UserExercise
			{
				ExerciseTypeId = 1, Version = 2, CreatedAt = date, UpdatedAt = date,
				UserId = users.First().UserId, ExerciseType = exerciseTypes.First()
			},
			// User exercise for non admin user
			new UserExercise
			{
				ExerciseTypeId = 2, Version = 2, CreatedAt = date, UpdatedAt = date,
				UserId = users.Last().UserId, ExerciseType = exerciseTypes[1]
			}
		];
	}

	/// <summary>
	///     TestData data for WorkoutSession
	///     It must be the third seed data to be inserted from this category
	///     It depends on UserExerciseSeedData.UserExercises
	/// </summary>
	/// <returns> List of WorkoutSession</returns>
	public static List<WorkoutSession> WorkoutSessions(AppDbContext context)
	{
		var users = UsersDataLoader.Users().ToList();
		var userExercises = context.UserExercises.ToList();

		if (users.Count == 0)
		{
			throw new Exception("User  data is required to seed UserExercise data");
		}

		if (userExercises.Count == 0)
		{
			throw new Exception("UserExercise  data is required to seed WorkoutSession data");
		}

		var firstUser = users.First().UserId;
		var firstUserExercises = userExercises.Where(e => e.UserId == firstUser).ToList();

		var lastUser = users.Last().UserId;
		var lastUserExercises = userExercises.Where(e => e.UserId == lastUser).ToList();

		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		return
		[
			// Workout sessions for admin user
			new WorkoutSession
			{
				SortOrder = 1, CreatedAt = date, UpdatedAt = date,
				UserId = firstUser,
				Name = "Test Workout Session 1",
				DayOfWeek = DayOfWeek.Monday,
				UserExercises = firstUserExercises
			},
			new WorkoutSession
			{
				SortOrder = 2, CreatedAt = date, UpdatedAt = date,
				UserId = firstUser,
				Name = "Test Workout Session 2",
				DayOfWeek = DayOfWeek.Tuesday,
				UserExercises = firstUserExercises
			},
			new WorkoutSession
			{
				SortOrder = 3, CreatedAt = date, UpdatedAt = date,
				UserId = firstUser,
				Name = "Test Workout Session 3",
				DayOfWeek = DayOfWeek.Wednesday,
				UserExercises = firstUserExercises
			},
			// Workout session for non admin user
			new WorkoutSession
			{
				SortOrder = 1, CreatedAt = date, UpdatedAt = date,
				UserId = lastUser,
				Name = "Test Workout Session 4",
				DayOfWeek = DayOfWeek.Monday,
				UserExercises = lastUserExercises
			}
		];
	}

	/// <summary>
	///     TestData data for UserExerciseHistory
	///     It must be the fourth seed data to be inserted from this category
	///     It depends on UserExerciseSeedData.UserExercises
	/// </summary>
	/// <returns></returns>
	public static List<UserExerciseHistory> UserExerciseHistories(AppDbContext context)
	{
		var users = UsersDataLoader.Users().ToList();
		var userExercises = context.UserExercises.ToList();

		if (users.Count == 0)
		{
			throw new Exception("User  data is required to seed UserExercise data");
		}

		if (userExercises.Count == 0)
		{
			throw new Exception("UserExercise  data is required to seed WorkoutSession data");
		}

		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		return
		[
			// User Exercise Histories for admin user
			new UserExerciseHistory
			{
				Repetition = 5, Weight = 100,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises.First().Id,
				UserId = users.First().UserId
			},
			new UserExerciseHistory
			{
				Repetition = 5, Weight = 100,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises[1].Id,
				UserId = users.First().UserId
			},
			new UserExerciseHistory
			{
				Repetition = 5, Weight = 100,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises[2].Id,
				UserId = users.First().UserId
			},
			// User Exercise Histories for non admin user
			new UserExerciseHistory
			{
				Repetition = 5, Weight = 100,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises[3].Id,
				UserId = users.Last().UserId
			}
		];
	}
}

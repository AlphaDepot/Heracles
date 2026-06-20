using Heracles.Domain.Entities;
using Heracles.Persistence;

namespace Heracles.Api.IntegrationTests.DbTestData;

public abstract class TestUserExerciseDataLoader
{
	public static void Initialize(AppDbContext context)
	{
		SeedUserExercises(context);
		SeedWorkoutSessions(context);
		SeedUserExerciseHistory(context);
	}

	private static void SeedUserExercises(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.UserExercises.Any())
			return;

		context.UserExercises.AddRange(UserExercises(context));
		context.SaveChanges();
	}

	private static void SeedWorkoutSessions(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.WorkoutSessions.Any())
			return;

		context.WorkoutSessions.AddRange(WorkoutSessions(context));
		context.SaveChanges();
	}

	private static void SeedUserExerciseHistory(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.UserExerciseHistories.Any())
			return;

		context.UserExerciseHistories.AddRange(UserExerciseHistories(context));
		context.SaveChanges();
	}

	public static List<UserExercise> UserExercises(AppDbContext context)
	{
		var users = TestUsersDataLoader.Users();
		var exerciseTypes = context.ExerciseTypes.ToList();

		var date = new DateTime(2022, 1, 1).ToUniversalTime();

		return
		[
			new UserExercise { UserId = users[0].UserId, ExerciseTypeId = 1, ExerciseType = exerciseTypes[0], Version = 1, CreatedAt = date, UpdatedAt = date },
			new UserExercise { UserId = users[0].UserId, ExerciseTypeId = 2, ExerciseType = exerciseTypes[1], Version = 1, CreatedAt = date, UpdatedAt = date },
			new UserExercise { UserId = users[0].UserId, ExerciseTypeId = 3, ExerciseType = exerciseTypes[2], Version = 1, CreatedAt = date, UpdatedAt = date },
			new UserExercise { UserId = users[1].UserId, ExerciseTypeId = 1, ExerciseType = exerciseTypes[0], Version = 2, CreatedAt = date, UpdatedAt = date }
		];
	}

	public static List<WorkoutSession> WorkoutSessions(AppDbContext context)
	{
		var users = TestUsersDataLoader.Users();
		var userExercises = context.UserExercises.ToList();

		var date = new DateTime(2022, 1, 1).ToUniversalTime();

		return
		[
			new WorkoutSession
			{
				UserId = users[0].UserId,
				Name = "Session 1",
				DayOfWeek = DayOfWeek.Monday,
				SortOrder = 1,
				UserExercises = userExercises.Where(x => x.UserId == users[0].UserId).ToList(),
				CreatedAt = date,
				UpdatedAt = date
			}
		];
	}

	public static List<UserExerciseHistory> UserExerciseHistories(AppDbContext context)
	{
		var users = TestUsersDataLoader.Users();
		var userExercises = context.UserExercises.ToList();

		var date = new DateTime(2022, 1, 1).ToUniversalTime();

		return
		[
			new UserExerciseHistory
			{
				UserId = users[0].UserId,
				UserExerciseId = userExercises[0].Id,
				Repetition = 5,
				Weight = 100,
				CreatedAt = date,
				UpdatedAt = date
			}
		];
	}
}

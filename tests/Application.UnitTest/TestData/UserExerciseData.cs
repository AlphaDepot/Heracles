using Application.Features.UserExerciseHistories;
using Application.Features.UserExercises;
using Application.Features.WorkoutSessions;

namespace Application.UnitTest.TestData;

public static class UserExerciseData
{
	/// <summary>
	///     TestData data for UserExercise
	///     It must be the second seed data to be inserted from this category
	///     It depends on ExerciseSeedData.ExerciseTypesController and UserSeedData.Users
	/// </summary>
	/// <returns> List of UserExercise</returns>
	public static List<UserExercise> UserExercises()
	{
		var users = UserData.Users().ToList();
		//var exerciseTypes = context.ExerciseTypes.ToList();
		var exerciseTypes = ExerciseTypeData.ExerciseTypes().ToList();

		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		var concurrency = Guid.NewGuid().ToString();
		return
		[
			// User exercises for admin user
			new UserExercise
			{
				Id = 1, ExerciseTypeId = 1, Version = 1, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				UserId = users.First().UserId, ExerciseType = exerciseTypes.First()
			},
			new UserExercise
			{
				Id = 2, ExerciseTypeId = 2, Version = 1, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				UserId = users.First().UserId, ExerciseType = exerciseTypes[1]
			},
			new UserExercise
			{
				Id = 3, ExerciseTypeId = 3, Version = 1, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				UserId = users.First().UserId, ExerciseType = exerciseTypes[2]
			},
			new UserExercise
			{
				Id = 4, ExerciseTypeId = 1, Version = 2, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				UserId = users.First().UserId, ExerciseType = exerciseTypes.First()
			},
			// User exercise for non admin user
			new UserExercise
			{
				Id = 5, ExerciseTypeId = 2, Version = 2, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
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
	public static List<WorkoutSession> WorkoutSessions()
	{
		var users = UserData.Users().ToList();
		var userExercises = UserExercises();


		var firstUser = users.First().UserId;
		var firstUserExercises = userExercises.Where(e => e.UserId == firstUser).ToList();

		var lastUser = users.Last().UserId;
		var lastUserExercises = userExercises.Where(e => e.UserId == lastUser).ToList();


		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		var concurrency = Guid.NewGuid().ToString();
		return
		[
			// Workout sessions for admin user
			new WorkoutSession
			{
				Id = 1, SortOrder = 1, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				UserId = firstUser,
				Name = "Test Workout Session 1",
				DayOfWeek = DayOfWeek.Monday,
				UserExercises = firstUserExercises
			},
			new WorkoutSession
			{
				Id = 2, SortOrder = 2, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				UserId = firstUser,
				Name = "Test Workout Session 2",
				DayOfWeek = DayOfWeek.Tuesday,
				UserExercises = firstUserExercises
			},
			new WorkoutSession
			{
				Id = 3, SortOrder = 3, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				UserId = firstUser,
				Name = "Test Workout Session 3",
				DayOfWeek = DayOfWeek.Wednesday,
				UserExercises = firstUserExercises
			},
			// Workout session for non admin user
			new WorkoutSession
			{
				Id = 4, SortOrder = 1, CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
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
	public static List<UserExerciseHistory> UserExerciseHistories()
	{
		var users = UserData.Users().ToList();
		var userExercises = UserExercises();

		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		var concurrency = Guid.NewGuid().ToString();
		return
		[
			// User Exercise Histories with admin user
			new UserExerciseHistory
			{
				Id = 1, Repetition = 5, Weight = 100, Concurrency = concurrency,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises.First().Id,
				UserId = users.First().UserId
			},

			new UserExerciseHistory
			{
				Id = 2, Repetition = 5, Weight = 100, Concurrency = concurrency,
				CreatedAt = date + TimeSpan.FromDays(1),
				UpdatedAt = date + TimeSpan.FromDays(2),
				UserExerciseId = userExercises.First().Id,
				UserId = users.First().UserId
			},

			new UserExerciseHistory
			{
				Id = 3, Repetition = 5, Weight = 100, Concurrency = concurrency,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises[1].Id,
				UserId = users.First().UserId
			},
			new UserExerciseHistory
			{
				Id = 4, Repetition = 5, Weight = 100, Concurrency = concurrency,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises[2].Id,
				UserId = users.First().UserId
			},
			// User Exercise Histories with non-admin user
			new UserExerciseHistory
			{
				Id = 5, Repetition = 5, Weight = 100, Concurrency = concurrency,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises[3].Id,
				UserId = users.Last().UserId
			},

			new UserExerciseHistory
			{
				Id = 6, Repetition = 5, Weight = 100, Concurrency = concurrency,
				CreatedAt = date,
				UpdatedAt = date,
				UserExerciseId = userExercises[3].Id,
				UserId = users.Last().UserId
			}
		];
	}
}

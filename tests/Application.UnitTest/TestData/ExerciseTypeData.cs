using Application.Features.ExerciseMuscleGroups;
using Application.Features.ExerciseTypes;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleGroups;

namespace Application.UnitTest.TestData;

public static class ExerciseTypeData
{
	/// <summary>
	///     SeedData data for MuscleFunction
	///     It must be the first seed data to be inserted since it has no dependencies
	/// </summary>
	/// <returns> List of MuscleFunction</returns>
	public static List<MuscleFunction> MuscleFunctions()
	{
		var date = new DateTime(2022, 1, 1);
		//var concurrency = Guid.NewGuid().ToString();
		return
		[
			new MuscleFunction { Id = 1, Name = "Stabilizer", CreatedAt = date, UpdatedAt = date },
			new MuscleFunction { Id = 2, Name = "Agonist", CreatedAt = date, UpdatedAt = date },
			new MuscleFunction { Id = 3, Name = "Antagonist", CreatedAt = date, UpdatedAt = date },
			new MuscleFunction { Id = 4, Name = "Synergist", CreatedAt = date, UpdatedAt = date },
			new MuscleFunction { Id = 5, Name = "Fixator", CreatedAt = date, UpdatedAt = date }
		];
	}

	/// <summary>
	///     SeedData data for MuscleGroup
	///     It must be the second  seed data to be inserted since it has no dependencies
	/// </summary>
	/// <returns> List of MuscleGroup</returns>
	public static List<MuscleGroup> MuscleGroups()
	{
		var date = new DateTime(2022, 1, 1);
		var concurrency = Guid.NewGuid().ToString();
		return
		[
			new MuscleGroup { Id = 1, Name = "Chest", CreatedAt = date, UpdatedAt = date, Concurrency = concurrency },
			new MuscleGroup { Id = 2, Name = "Back", CreatedAt = date, UpdatedAt = date, Concurrency = concurrency },
			new MuscleGroup { Id = 3, Name = "Legs", CreatedAt = date, UpdatedAt = date, Concurrency = concurrency },
			new MuscleGroup
				{ Id = 4, Name = "Hamstrings", CreatedAt = date, UpdatedAt = date, Concurrency = concurrency },
			new MuscleGroup { Id = 5, Name = "Calves", CreatedAt = date, UpdatedAt = date, Concurrency = concurrency }
		];
	}

	/// <summary>
	///     SeedData data for ExerciseMuscleGroup
	///     It must be the third seed data to be inserted
	///     It depends on  MuscleGroup and MuscleFunction
	/// </summary>
	/// <returns> List of ExerciseMuscleGroup</returns>
	public static List<ExerciseMuscleGroup> ExerciseMuscleGroups()
	{
		var date = new DateTime(2022, 1, 1);
		var concurrency = Guid.NewGuid().ToString();
		return
		[
			// first [0] or .First()
			new ExerciseMuscleGroup
			{
				Id = 1, ExerciseTypeId = 1, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				Muscle = MuscleGroups().First(),
				Function = MuscleFunctions().First()
			},
			// second [1]
			new ExerciseMuscleGroup
			{
				Id = 2, ExerciseTypeId = 2, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				Muscle = MuscleGroups()[1],
				Function = MuscleFunctions()[1]
			},
			// third [2]
			new ExerciseMuscleGroup
			{
				Id = 3, ExerciseTypeId = 3, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				Muscle = MuscleGroups()[2],
				Function = MuscleFunctions()[2]
			},
			// fourth [3]
			new ExerciseMuscleGroup
			{
				Id = 4, ExerciseTypeId = 1, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				Muscle = MuscleGroups()[3],
				Function = MuscleFunctions()[3]
			},
			// fifth [4]
			new ExerciseMuscleGroup
			{
				Id = 5, ExerciseTypeId = 1, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				Muscle = MuscleGroups()[4],
				Function = MuscleFunctions()[4]
			}
		];
	}

	/// <summary>
	///     SeedData data for ExerciseType
	///     It must be the fourth seed data to be inserted
	///     It depends on ExerciseMuscleGroup which depends on MuscleGroup and MuscleFunction
	/// </summary>
	/// <returns> List of ExerciseType</returns>
	public static List<ExerciseType> ExerciseTypes()
	{
		var date = new DateTime(2022, 1, 1);
		var concurrency = Guid.NewGuid().ToString();
		return
		[
			new ExerciseType
			{
				Id = 1, Name = "Bench Press",
				CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				Description = "Lay on a bench and press the bar",
				MuscleGroups =
				[
					ExerciseMuscleGroups().First(),
					ExerciseMuscleGroups()[3],
					ExerciseMuscleGroups()[4]
				]
			},
			new ExerciseType
			{
				Id = 2, Name = "Squat",
				CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				Description = "Squat down and stand back up",
				MuscleGroups =
				[
					ExerciseMuscleGroups()[2]
				]
			},
			new ExerciseType
			{
				Id = 3, Name = "Deadlift",
				CreatedAt = date, UpdatedAt = date, Concurrency = concurrency,
				Description = "Lift the bar from the ground",
				MuscleGroups =
				[
					ExerciseMuscleGroups()[1]
				]
			}
		];
	}
}

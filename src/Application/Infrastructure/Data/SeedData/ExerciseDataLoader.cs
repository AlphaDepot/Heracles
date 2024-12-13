using Application.Features.ExerciseMuscleGroups;
using Application.Features.ExerciseTypes;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleGroups;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Data.SeedData;

internal abstract class ExerciseDataLoader
{
	public static void Initialize(AppDbContext context)
	{
		// !!! ORDER OF SEED DATA IS IMPORTANT!!! #1#

		// TestData MuscleGroup first since it has no dependencies
		SeedMuscleGroup(context);
		// TestData MuscleFunction second since it has no dependencies
		SeedMuscleFunction(context);
		// TestData ExerciseType third without dependencies
		SeedExerciseType(context);
		// TestData ExerciseMuscleGroup fourth since it depends on MuscleGroup and MuscleFunction
		SeedExerciseMuscleGroup(context);
		// Update ExerciseTypesController with ExerciseMuscleGroups 
		UpdateExerciseTypesWithExerciseMuscleGroups(context);
	}


	/// <summary>
	///     TestData the MuscleGroup data into the database.
	/// </summary>
	/// <param name="context">The database context.</param>
	private static void SeedMuscleGroup(AppDbContext context)
	{
		// ensure the database is created 
		context.Database.EnsureCreated();

		// check if the database is already seeded
		if (context.MuscleGroups.Any())
		{
			return;
		}

		// TestData data
		var muscleGroups = MuscleGroups();

		// Add the seed data to the database
		context.MuscleGroups.AddRange(muscleGroups);

		// Save the changes
		context.SaveChanges();
	}

	/// <summary>
	///     TestData the MuscleFunction data into the database.
	/// </summary>
	/// <param name="context"></param>
	private static void SeedMuscleFunction(AppDbContext context)
	{
		// ensure the database is created 
		context.Database.EnsureCreated();

		// check if the database is already seeded
		if (context.MuscleFunctions.Any())
		{
			return;
		}

		// TestData data
		var muscleFunctions = MuscleFunctions();

		// Add the seed data to the database
		context.MuscleFunctions.AddRange(muscleFunctions);

		// Save the changes
		context.SaveChanges();
	}

	/// <summary>
	///     TestData the ExerciseMuscleGroup data into the database.
	/// </summary>
	/// <param name="context">The database context.</param>
	private static void SeedExerciseMuscleGroup(AppDbContext context)
	{
		// ensure the database is created 
		context.Database.EnsureCreated();

		// check if the database is already seeded
		if (context.ExerciseMuscleGroups.Any())
		{
			return;
		}

		// TestData data
		var exerciseMuscleGroups = ExerciseMuscleGroups(context);

		// Add the seed data to the database
		context.ExerciseMuscleGroups.AddRange(exerciseMuscleGroups);

		// Save the changes
		context.SaveChanges();
	}


	/// <summary>
	///     TestData the ExerciseType data into the database.
	/// </summary>
	/// <param name="context">The database context.</param>
	private static void SeedExerciseType(AppDbContext context)
	{
		// ensure the database is created 
		context.Database.EnsureCreated();

		// check if the database is already seeded
		if (context.ExerciseTypes.Any())
		{
			return;
		}

		// TestData data
		var exerciseTypes = ExerciseTypes();

		// Add the seed data to the database
		context.ExerciseTypes.AddRange(exerciseTypes);

		// Save the changes
		context.SaveChanges();
	}

	/// <summary>
	///     Updates the ExerciseTypesController with ExerciseMuscleGroups.
	/// </summary>
	/// <param name="context">The database context.</param>
	public static void UpdateExerciseTypesWithExerciseMuscleGroups(AppDbContext context)
	{
		var exerciseTypes = context.ExerciseTypes.Include(et => et.MuscleGroups).ToList();
		var exerciseMuscleGroups = context.ExerciseMuscleGroups.ToList();

		foreach (var exerciseType in exerciseTypes)
		{
			exerciseType.MuscleGroups =
				exerciseMuscleGroups.Where(emg => emg.ExerciseTypeId == exerciseType.Id).ToList();
		}

		context.SaveChanges();
	}

	/// <summary>
	///     TestData data for MuscleFunction
	///     It must be the first seed data to be inserted since it has no dependencies
	/// </summary>
	/// <returns> List of MuscleFunction</returns>
	public static List<MuscleFunction> MuscleFunctions()
	{
		// set a date for the created and updated at fields in UTC
		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		return
		[
			new MuscleFunction { Name = "Stabilizer", CreatedAt = date, UpdatedAt = date },
			new MuscleFunction { Name = "Agonist", CreatedAt = date, UpdatedAt = date },
			new MuscleFunction { Name = "Antagonist", CreatedAt = date, UpdatedAt = date },
			new MuscleFunction { Name = "Synergist", CreatedAt = date, UpdatedAt = date },
			new MuscleFunction { Name = "Fixator", CreatedAt = date, UpdatedAt = date }
		];
	}

	/// <summary>
	///     TestData data for MuscleGroup
	///     It must be the second  seed data to be inserted since it has no dependencies
	/// </summary>
	/// <returns> List of MuscleGroup</returns>
	public static List<MuscleGroup> MuscleGroups()
	{
		// set a date for the created and updated at fields in UTC
		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		return
		[
			new MuscleGroup { Name = "Chest", CreatedAt = date, UpdatedAt = date },
			new MuscleGroup { Name = "Back", CreatedAt = date, UpdatedAt = date },
			new MuscleGroup { Name = "Legs", CreatedAt = date, UpdatedAt = date },
			new MuscleGroup { Name = "Hamstrings", CreatedAt = date, UpdatedAt = date },
			new MuscleGroup { Name = "Calves", CreatedAt = date, UpdatedAt = date }
		];
	}

	/// <summary>
	///     TestData data for ExerciseMuscleGroup
	///     It must be the third seed data to be inserted
	///     It depends on  MuscleGroup and MuscleFunction
	/// </summary>
	/// <param name="context">The database context.</param>
	/// <returns> List of ExerciseMuscleGroup</returns>
	public static List<ExerciseMuscleGroup> ExerciseMuscleGroups(AppDbContext context)
	{
		// get the muscle groups and muscle functions from the database
		var muscleGroups = context.MuscleGroups.ToList();
		var muscleFunctions = context.MuscleFunctions.ToList();

		if (!muscleGroups.Any())
		{
			throw new InvalidOperationException(
				"MuscleGroups is empty. Ensure it is populated before seeding ExerciseMuscleGroups.");
		}

		if (!muscleFunctions.Any())
		{
			throw new InvalidOperationException(
				"MuscleFunctions is empty. Ensure it is populated before seeding ExerciseMuscleGroups.");
		}


		// set a date for the created and updated at fields in UTC
		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		return
		[
			// first [0] or .First()
			new ExerciseMuscleGroup
			{
				ExerciseTypeId = 1, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date,
				Muscle = muscleGroups.First(),
				Function = muscleFunctions.First()
			},
			// second [1]
			new ExerciseMuscleGroup
			{
				ExerciseTypeId = 2, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date,
				Muscle = muscleGroups[1],
				Function = muscleFunctions[1]
			},
			// third [2]
			new ExerciseMuscleGroup
			{
				ExerciseTypeId = 3, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date,
				Muscle = muscleGroups[2],
				Function = muscleFunctions[2]
			},
			// fourth [3]
			new ExerciseMuscleGroup
			{
				ExerciseTypeId = 1, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date,
				Muscle = muscleGroups[3],
				Function = muscleFunctions[3]
			},
			// fifth [4]
			new ExerciseMuscleGroup
			{
				ExerciseTypeId = 1, FunctionPercentage = 100,
				CreatedAt = date, UpdatedAt = date,
				Muscle = muscleGroups[4],
				Function = muscleFunctions[4]
			}
		];
	}


	/// <summary>
	///     TestData data for ExerciseType
	///     It must be the fourth seed data to be inserted
	///     It depends on ExerciseMuscleGroup which depends on MuscleGroup and MuscleFunction
	/// </summary>
	/// <returns> List of ExerciseType</returns>
	public static List<ExerciseType> ExerciseTypes()
	{
		// set a date for the created and updated at fields in UTC
		var date = new DateTime(2022, 1, 1).ToUniversalTime();
		return
		[
			new ExerciseType
			{
				Name = "Bench Press",
				CreatedAt = date, UpdatedAt = date,
				Description = "Lay on a bench and press the bar",
				ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/e/ea/Decline-bench-press-2.png"
			},
			new ExerciseType
			{
				Name = "Squat",
				CreatedAt = date, UpdatedAt = date,
				Description = "Squat down and stand back up",
				ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/0/0f/Wide-stance-squat-1.gif"
			},
			new ExerciseType
			{
				Name = "Deadlift",
				CreatedAt = date, UpdatedAt = date,
				Description = "Lift the bar from the ground",
				ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/5/58/Romanian-deadlift-2.png"
			}
		];
	}
}

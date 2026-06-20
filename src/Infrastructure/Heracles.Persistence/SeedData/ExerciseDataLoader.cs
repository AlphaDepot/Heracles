using Heracles.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.SeedData;

internal abstract class ExerciseDataLoader
{
	public static void Initialize(AppDbContext context)
	{
		SeedMuscleGroup(context);
		SeedMuscleFunction(context);
		SeedExerciseType(context);
		SeedExerciseMuscleGroup(context);
		UpdateExerciseTypesWithExerciseMuscleGroups(context);
	}

	private static void SeedMuscleGroup(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.MuscleGroups.Any())
		{
			return;
		}

		context.MuscleGroups.AddRange(MuscleGroups());
		context.SaveChanges();
	}

	private static void SeedMuscleFunction(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.MuscleFunctions.Any())
		{
			return;
		}

		context.MuscleFunctions.AddRange(MuscleFunctions());
		context.SaveChanges();
	}

	private static void SeedExerciseType(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.ExerciseTypes.Any())
		{
			return;
		}

		context.ExerciseTypes.AddRange(ExerciseTypes());
		context.SaveChanges();
	}

	private static void SeedExerciseMuscleGroup(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.ExerciseMuscleGroups.Any())
		{
			return;
		}

		context.ExerciseMuscleGroups.AddRange(ExerciseMuscleGroups(context));
		context.SaveChanges();
	}

	public static void UpdateExerciseTypesWithExerciseMuscleGroups(AppDbContext context)
	{
		var exerciseTypes = context.ExerciseTypes.Include(x => x.MuscleGroups).ToList();
		var emgs = context.ExerciseMuscleGroups.ToList();

		foreach (var et in exerciseTypes)
		{
			et.MuscleGroups = emgs.Where(x => x.ExerciseTypeId == et.Id).ToList();
		}

		context.SaveChanges();
	}

	public static List<MuscleFunction> MuscleFunctions()
	{
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

	public static List<MuscleGroup> MuscleGroups()
	{
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

	public static List<ExerciseType> ExerciseTypes()
	{
		var date = new DateTime(2022, 1, 1).ToUniversalTime();

		return
		[
			new ExerciseType
			{
				Name = "Bench Press",
				CreatedAt = date,
				UpdatedAt = date,
				Description = "Lay on a bench and press the bar",
				Images = ["https://upload.wikimedia.org/wikipedia/commons/e/ea/Decline-bench-press-2.png"]
			},
			new ExerciseType
			{
				Name = "Squat",
				CreatedAt = date,
				UpdatedAt = date,
				Description = "Squat down and stand back up",
				Images = ["https://upload.wikimedia.org/wikipedia/commons/0/0f/Wide-stance-squat-1.gif"]
			},
			new ExerciseType
			{
				Name = "Deadlift",
				CreatedAt = date,
				UpdatedAt = date,
				Description = "Lift the bar from the ground",
				Images = ["https://upload.wikimedia.org/wikipedia/commons/5/58/Romanian-deadlift-2.png"]
			}
		];
	}

	public static List<ExerciseMuscleGroup> ExerciseMuscleGroups(AppDbContext context)
	{
		var muscleGroups = context.MuscleGroups.ToList();
		var muscleFunctions = context.MuscleFunctions.ToList();

		if (!muscleGroups.Any() || !muscleFunctions.Any())
		{
			throw new InvalidOperationException("Missing muscle seed data.");
		}

		var date = new DateTime(2022, 1, 1).ToUniversalTime();

		return
		[
			new ExerciseMuscleGroup
			{
				ExerciseTypeId = 1,
				FunctionPercentage = 100,
				CreatedAt = date,
				UpdatedAt = date,
				Muscle = muscleGroups[0],
				Function = muscleFunctions[0]
			},
			new ExerciseMuscleGroup
			{
				ExerciseTypeId = 2,
				FunctionPercentage = 100,
				CreatedAt = date,
				UpdatedAt = date,
				Muscle = muscleGroups[1],
				Function = muscleFunctions[1]
			},
			new ExerciseMuscleGroup
			{
				ExerciseTypeId = 3,
				FunctionPercentage = 100,
				CreatedAt = date,
				UpdatedAt = date,
				Muscle = muscleGroups[2],
				Function = muscleFunctions[2]
			}
		];
	}
}

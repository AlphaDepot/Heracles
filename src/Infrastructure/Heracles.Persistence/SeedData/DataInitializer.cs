namespace Heracles.Persistence.SeedData;

public class DataInitializer
{
	public static void Initialize(AppDbContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		/* ORDER MATTERS */

		UsersDataLoader.Initialize(context);

		EquipmentDataLoader.Initialize(context);

		ExerciseDataLoader.Initialize(context);

		UserExerciseDataLoader.Initialize(context);
	}
}

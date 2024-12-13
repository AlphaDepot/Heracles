using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Data.SeedData;

public class DataInitializer
{
	/// <summary>
	///     Initializes the database with seed data for the Heracles application.
	/// </summary>
	/// <param name="context">The HeraclesDbContext to initialize the data.</param>
	public static void Initialize(AppDbContext context)
	{
		// check dbcontext is not null
		ArgumentNullException.ThrowIfNull(context, nameof(context));

		// Create or update the database
		context.Database.Migrate();

		/* !!! ORDER OF SEED DATA IS IMPORTANT!!! */

		// First load Equipment TestData Data
		EquipmentDataLoader.Initialize(context);

		// Second load Exercise TestData Data
		ExerciseDataLoader.Initialize(context);

		// Third load UserExercise TestData Data
		UserExerciseDataLoader.Initialize(context);

		// Load Users
		UsersDataLoader.Initialize(context);
	}
}

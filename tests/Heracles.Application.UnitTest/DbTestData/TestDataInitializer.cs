using Heracles.Persistence;
using Heracles.Persistence.SeedData;

namespace Heracles.Application.UnitTest.DbTestData;

public class TestDataInitializer
{
	public static void Initialize(AppDbContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		/* ORDER MATTERS */

		TestUsersDataLoader.Initialize(context);

		TestEquipmentDataLoader.Initialize(context);

		TestExerciseDataLoader.Initialize(context);

		TestUserExerciseDataLoader.Initialize(context);
	}
}

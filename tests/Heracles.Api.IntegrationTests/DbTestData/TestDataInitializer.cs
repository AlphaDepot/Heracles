using Heracles.Persistence;
using Heracles.Persistence.SeedData;

namespace Heracles.Api.IntegrationTests.DbTestData;

public class TestDataInitializer
{
	public static void Initialize(AppDbContext context)
	{
		ArgumentNullException.ThrowIfNull(context, nameof(context));

		/* ORDER MATTERS */

		TestUsersDataLoader.Initialize(context);

		TestEquipmentDataLoader.Initialize(context);

		TestExerciseDataLoader.Initialize(context);

		TestUserExerciseDataLoader.Initialize(context);
	}
}

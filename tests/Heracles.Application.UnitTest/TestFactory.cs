using Heracles.Application.UnitTest.DbTestData;
using Heracles.Persistence;
using Heracles.Persistence.Repositories;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Repositories.Base;
using Heracles.Shared.Interfaces.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Application.UnitTest;

public static class TestFactory
{
	public static ServiceProvider Create(ICurrentUserService currentUserService)
	{
		var services = new ServiceCollection();

		var connection = new SqliteConnection("DataSource=:memory:");
		connection.Open();

		services.AddSingleton(connection);

		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlite(connection);

			options.ConfigureWarnings(w =>
				w.Ignore(RelationalEventId.PendingModelChangesWarning));
		});

		services.AddScoped(_ => currentUserService);

		services.AddTestPersistenceServices();

		var provider = services.BuildServiceProvider();

		// 🔥 CRITICAL: DO NOT USE SCOPE HERE
		var context = provider.GetRequiredService<AppDbContext>();

		context.Database.EnsureDeleted();
		context.Database.EnsureCreated();

		TestDataInitializer.Initialize(context);

		return provider;
	}

	public static IServiceCollection AddTestPersistenceServices(this IServiceCollection services)
	{
		services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

		services.AddScoped<IEquipmentGroupRepository, EquipmentGroupRepository>();
		services.AddScoped<IEquipmentRepository, EquipmentRepository>();
		services.AddScoped<IExerciseMuscleGroupsRepository, ExerciseMuscleGroupRepository>();
		services.AddScoped<IExerciseTypesRepository, ExerciseTypeRepository>();
		services.AddScoped<IMuscleFunctionsRepository, MuscleFunctionRepository>();
		services.AddScoped<IMuscleGroupsRepository, MuscleGroupRepository>();
		services.AddScoped<IUserExerciseHistoriesRepository, UserExerciseHistoryRepository>();
		services.AddScoped<IUserExercisesRepository, UserExerciseRepository>();
		services.AddScoped<IUsersRepository, UserRepository>();
		services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();

		return services;
	}
}

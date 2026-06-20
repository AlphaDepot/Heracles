using Heracles.Persistence.Repositories;
using Heracles.Persistence.Repositories.Base;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Persistence;

public static class PersistenceServiceRegistration
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
		IConfiguration configuration)
	{
		// Setup Database
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
		});

		// Register Repositories
		services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
		services.AddScoped(typeof(INamedRepository<>), typeof(NamedRepository<>));
		services.AddScoped(typeof(ITypeRepository<>), typeof(TypeRepository<>));
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

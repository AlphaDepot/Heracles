using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.WorkoutSessions.Interfaces;
using Heracles.Persistence.DataContext;
using Heracles.Persistence.Repository;
using Heracles.Persistence.SeedData;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<HeraclesDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        
        
        
        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IExerciseTypeRepository, ExerciseTypeRepository>();
        services.AddScoped<IExerciseMuscleGroupRepository , ExerciseMuscleGroupRepository>();
        services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
        services.AddScoped<IMuscleFunctionRepository , MuscleFunctionRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IEquipmentGroupRepository, EquipmentGroupRepository>();
        services.AddScoped<IUserExerciseRepository, UserExerciseRepository>();
        services.AddScoped<IUserExerciseHistoryRepository, UserExerciseHistoryRepository>();
        services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
    
    
    public static IApplicationBuilder LoadSeedData(this IApplicationBuilder app)
    {
        // Check if the app is null and throw an ArgumentNullException if it is
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        
        // Create a scope to get the service provider
        using var scope = app.ApplicationServices.CreateScope();
        // Get the service provider
        var services = scope.ServiceProvider;
       
        
        // Try to seed the database
        try 
        {
            // Get the HeraclesDbContext
            var context = services.GetRequiredService<HeraclesDbContext>();
            DataInitializer.Initialize(context);
            
        }
        catch (Exception ex)
        {
           // var logger = services.GetRequiredService<ILogger<Program>>();
           var logger = services.GetRequiredService<IAppLogger<DataInitializer>>();
            logger.LogError($"An error occurred while seeding the database. {ex}", ex);
        }
        
        return app;
    }
}
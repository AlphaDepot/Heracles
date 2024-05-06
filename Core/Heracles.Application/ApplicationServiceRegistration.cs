using Heracles.Application.Features.EquipmentGroups;
using Heracles.Application.Features.Equipments;
using Heracles.Application.Features.ExerciseMuscleGroups;
using Heracles.Application.Features.ExerciseTypes;
using Heracles.Application.Features.MuscleFunctions;
using Heracles.Application.Features.MuscleGroups;
using Heracles.Application.Features.UserExercises;
using Heracles.Application.Features.UserExercisesHistories;
using Heracles.Application.Features.Users;
using Heracles.Application.Features.WorkoutSessions;
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
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add services to the container.
        services.AddHttpContextAccessor();
        
        // Add services to the container.
        services.AddScoped<IEquipmentGroupService, EquipmentGroupService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IExerciseMuscleGroupService, ExerciseMuscleGroupService>();
        services.AddScoped<IExerciseTypeService, ExerciseTypeService>();
        services.AddScoped<IMuscleFunctionService, MuscleFunctionService>();
        services.AddScoped<IMuscleGroupService, MuscleGroupService>();
        services.AddScoped<IUserExerciseHistoryService, UserExerciseHistoryService>();
        services.AddScoped<IUserExerciseService, UserExerciseService>();
        services.AddScoped<IWorkoutSessionService, WorkoutSessionService>();
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}


using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Domain.UserExercises.Models;
using Heracles.Domain.Users.Models;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.Persistence.DataContext;

namespace Heracles.Persistence.SeedData;

public abstract class UserExerciseDataLoader
{
    
    public static void Initialize(HeraclesDbContext context)
    {
        // !!! ORDER OF SEED DATA IS IMPORTANT!!! #1#
        
        // SeedData Users first since it has no dependencies
        SeedUsers(context);
        
        // SeedData UserExercises second since it depends on Users
        SeedUserExercises(context);
        
        // SeedData WorkoutSessions third since it depends on UserExercises
        SeedWorkoutSessions(context);
        
        // SeedData UserExerciseHistories fourth since it depends on UserExercises
        SeedUserExerciseHistory(context);

    }

    /// <summary>
    /// SeedData users into the database.
    /// </summary>
    /// <param name="context">The database context to seed the users into.</param>
    private static void SeedUsers(HeraclesDbContext context)
    {
        // ensure the database is created 
        context.Database.EnsureCreated();
        
        // check if the database is already seeded
        if (context.Users.Any())  return;
        
        // SeedData data
        var users = Users();
        
        // Add the seed data to the database
        context.Users.AddRange(users);
        
        // Save the changes
        context.SaveChanges();
        
    }

    /// <summary>
    /// SeedData user exercises into the database.
    /// </summary>
    /// <param name="context">The database context to seed the user exercises into.</param>
    private static void SeedUserExercises(HeraclesDbContext context)
    {
        // ensure the database is created 
        context.Database.EnsureCreated();
        
        // check if the database is already seeded
        if (context.UserExercises.Any())  return;
        
        // SeedData data
        var userExercises = UserExercises(context);
        
        // Add the seed data to the database
        context.UserExercises.AddRange(userExercises);
        
        // Save the changes
        context.SaveChanges();
    }

    /// <summary>
    /// SeedData workout sessions into the database.
    /// </summary>
    /// <param name="context">The database context to seed the workout sessions into.</param>
    private static void SeedWorkoutSessions(HeraclesDbContext context)
    {
        // ensure the database is created 
        context.Database.EnsureCreated();
        
        // check if the database is already seeded
        if (context.WorkoutSessions.Any())  return;
        
        // SeedData data
        var workoutSessions = WorkoutSessions(context);
        
        // Add the seed data to the database
        context.WorkoutSessions.AddRange(workoutSessions);
        
        // Save the changes
        context.SaveChanges();
    }


    /// <summary>
    /// SeedData user exercise history data into the database.
    /// </summary>
    /// <param name="context">The database context to seed the data into.</param>
    private static void SeedUserExerciseHistory(HeraclesDbContext context)
    {
        // ensure the database is created 
        context.Database.EnsureCreated();
        
        // check if the database is already seeded
        if (context.UserExerciseHistories.Any())  return;
        
        // SeedData data
        var userExerciseHistories = UserExerciseHistories(context);
        
        // Add the seed data to the database
        context.UserExerciseHistories.AddRange(userExerciseHistories);
        
        // Save the changes
        context.SaveChanges();
    }
    
    /// <summary>
    ///  SeedData data for User
    ///  It must be the first seed data to be inserted from this category
    ///  No dependencies
    /// </summary>
    /// <returns></returns>
    public static List<User> Users()
    {
        var date = new DateTime(2022, 1, 1).ToUniversalTime();
        return
        [
            // admin user
            new User {  UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f21",
                Name = "Admin Test User", Email = "admin.test.user@test.com",
                CreatedAt =  date,
                UpdatedAt = date,
                Roles = new List<string> { "Admin" },
                LastLogin = date 
            },
            // non admin user
            new User { UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f25",
                CreatedAt =  date, 
                UpdatedAt = date,
                Name = "Test User", Email = "test.user@test.com",
                LastLogin = date 
            },
             new User {  UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f26",
                CreatedAt =  date, 
                UpdatedAt = date,
                Name = "Test User 2", Email = "test.user.2@test.com",
                LastLogin = date 
            }
        ];
    }

    /// <summary>
    ///  SeedData data for UserExercise
    ///  It must be the second seed data to be inserted from this category
    ///  It depends on ExerciseSeedData.ExerciseTypes and UserSeedData.Users
    /// </summary>
    /// <returns> List of UserExercise</returns>
    public static List<UserExercise> UserExercises(HeraclesDbContext context)
    {
        
        var users = context.Users.ToList();
        var exerciseTypes = context.ExerciseTypes.ToList();
        
        if (users.Count == 0 )
            throw new Exception("User  data is required to seed UserExercise data");
        
        if (exerciseTypes.Count == 0 )
            throw new Exception("ExerciseType data is required to seed UserExercise data");
        
        
        var date = new DateTime(2022, 1, 1).ToUniversalTime();
        return
        [
            // User exercises for admin user
            new UserExercise {  ExerciseTypeId = 1, Version = 1, CreatedAt =  date, UpdatedAt = date,
                UserId = users.First().UserId, ExerciseType = exerciseTypes.First() },
            new UserExercise { ExerciseTypeId = 2, Version = 1, CreatedAt =  date, UpdatedAt = date,
                UserId = users.First().UserId, ExerciseType = exerciseTypes[1] },
            new UserExercise {   ExerciseTypeId = 3, Version = 1, CreatedAt =  date, UpdatedAt = date,
                UserId = users.First().UserId, ExerciseType = exerciseTypes[2] },
            new UserExercise {  ExerciseTypeId = 1, Version = 2, CreatedAt =  date, UpdatedAt = date,
                UserId = users.First().UserId, ExerciseType = exerciseTypes.First() },
            // User exercise for non admin user
            new UserExercise {   ExerciseTypeId = 2, Version = 2, CreatedAt =  date, UpdatedAt = date,
                UserId = users.Last().UserId, ExerciseType = exerciseTypes[1] },
        ];
    }
    
    /// <summary>
    ///  SeedData data for WorkoutSession
    ///  It must be the third seed data to be inserted from this category
    ///  It depends on UserExerciseSeedData.UserExercises
    /// </summary>
    /// <returns> List of WorkoutSession</returns>
    public static List<WorkoutSession> WorkoutSessions(HeraclesDbContext context)
    {
        var users = context.Users.ToList();
        var userExercises = context.UserExercises.ToList();
        
        if (users.Count == 0 )
            throw new Exception("User  data is required to seed UserExercise data");

        if (userExercises.Count == 0 )
            throw new Exception("UserExercise  data is required to seed WorkoutSession data");
        
        var firstUser =  users.First().UserId;
        var firstUserExercises = userExercises.Where(e=> e.UserId == firstUser).ToList();
        
        var lastUser = users.Last().UserId;
        var lastUserExercises = userExercises.Where(e=> e.UserId == lastUser).ToList();
        
        var date = new DateTime(2022, 1, 1).ToUniversalTime();
        return
        [
            // Workout sessions for admin user
            new WorkoutSession {  SortOrder = 1, CreatedAt =  date, UpdatedAt = date,
                UserId = firstUser,
                Name = "Test Workout Session 1",
                DayOfWeek = DayOfWeek.Monday,
                UserExercises = firstUserExercises
            },
            new WorkoutSession {  SortOrder = 2, CreatedAt =  date, UpdatedAt = date,
                UserId = firstUser,
                Name = "Test Workout Session 2",
                DayOfWeek = DayOfWeek.Tuesday,
                UserExercises = firstUserExercises
            },
            new WorkoutSession {  SortOrder = 3, CreatedAt =  date, UpdatedAt = date,
                UserId = firstUser,
                Name = "Test Workout Session 3",
                DayOfWeek = DayOfWeek.Wednesday,
                UserExercises = firstUserExercises
            },
            // Workout session for non admin user
            new WorkoutSession {  SortOrder = 1, CreatedAt =  date, UpdatedAt = date,
                UserId = lastUser,
                Name = "Test Workout Session 4",
                DayOfWeek = DayOfWeek.Monday,
                UserExercises = lastUserExercises
            },
        ];
    }

    /// <summary>
    ///  SeedData data for UserExerciseHistory
    ///  It must be the fourth seed data to be inserted from this category
    ///   It depends on UserExerciseSeedData.UserExercises
    /// </summary>
    /// <returns></returns>
    public static List<UserExerciseHistory> UserExerciseHistories(HeraclesDbContext context)
    {
        var users = context.Users.ToList();
        var userExercises = context.UserExercises.ToList();
        
        if (users.Count == 0 )
            throw new Exception("User  data is required to seed UserExercise data");

        if (userExercises.Count == 0 )
            throw new Exception("UserExercise  data is required to seed WorkoutSession data");
        
        var date = new DateTime(2022, 1, 1).ToUniversalTime();
        return
        [
            // User Exercise Histories for admin user
            new UserExerciseHistory {  Repetition = 5, Weight = 100,
                CreatedAt =  date, 
                UpdatedAt = date,
                UserExerciseId =userExercises.First().Id,
                UserId = users.First().UserId
            },
            new UserExerciseHistory {  Repetition = 5, Weight = 100,
                CreatedAt =  date, 
                UpdatedAt = date,
                UserExerciseId = userExercises[1].Id,
                UserId = users.First().UserId
            },
            new UserExerciseHistory {  Repetition = 5, Weight = 100,
                CreatedAt =  date, 
                UpdatedAt = date,
                UserExerciseId = userExercises[2].Id,
                UserId = users.First().UserId
            },
            // User Exercise Histories for non admin user
            new UserExerciseHistory {  Repetition = 5, Weight = 100,
                CreatedAt =  date, 
                UpdatedAt = date,
                UserExerciseId = userExercises[3].Id,
                UserId = users.Last().UserId
            },
            
        ];
    }
    
}
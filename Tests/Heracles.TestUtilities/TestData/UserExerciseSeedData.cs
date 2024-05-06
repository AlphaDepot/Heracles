using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Domain.UserExercises.Models;
using Heracles.Domain.Users.Models;
using Heracles.Domain.WorkoutSessions.Models;

namespace Heracles.TestUtilities.TestData;

/// <summary>
///   SeedData data for User related entities
///  The ExerciseSeedData must be the first seed data to be inserted
/// </summary>
public static class UserExerciseSeedData
{
    /// <summary>
    ///  SeedData data for User
    ///  It must be the first seed data to be inserted from this category
    ///  No dependencies
    /// </summary>
    /// <returns></returns>
    public static List<User> Users()
    {
        var date = new DateTime(2022, 1, 1);
        return
        [
            // admin user
            new User { Id = 1, UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f21",
                Name = "Admin Test User", Email = "admin.test.user@test.com",
                CreatedAt = date, UpdatedAt = date,
                Roles = new List<string> { "Admin" },
                LastLogin = new DateTime(2022, 1, 1) // 1st January 2022 00:00:00 AM UTC - Require fixed date for testing
            },
            // non admin user
            new User { Id = 2, UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f25",
                CreatedAt = date, UpdatedAt = date,
                Name = "Test User", Email = "test.user@test.com",
                LastLogin = new DateTime(2022, 1, 1) // 1st January 2022 00:00:00 AM UTC - Require fixed date for testing
            },
             new User { Id = 3, UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f26",
                CreatedAt = date, UpdatedAt = date,
                Name = "Test User 2", Email = "test.user.2@test.com",
                LastLogin = new DateTime(2022, 1, 1) // 1st January 2022 00:00:00 AM UTC - Require fixed date for testing
            }
        ];
    }

    /// <summary>
    ///  SeedData data for UserExercise
    ///  It must be the second seed data to be inserted from this category
    ///  It depends on ExerciseSeedData.ExerciseTypes and UserSeedData.Users
    /// </summary>
    /// <returns> List of UserExercise</returns>
    public static List<UserExercise> UserExercises()
    {
        var date = new DateTime(2022, 1, 1);
        return
        [
            // User exercises for admin user
            new UserExercise { Id = 1,  ExerciseTypeId = 1, Version = 1, CreatedAt = date, UpdatedAt = date,
                UserId = Users().First()!.UserId, ExerciseType = ExerciseSeedData.ExerciseTypes().First() },
            new UserExercise { Id = 2,  ExerciseTypeId = 2, Version = 1, CreatedAt = date, UpdatedAt = date,
                UserId = Users().First()!.UserId, ExerciseType = ExerciseSeedData.ExerciseTypes()[1] },
            new UserExercise { Id = 3,  ExerciseTypeId = 3, Version = 1, CreatedAt = date, UpdatedAt = date,
                UserId = Users().First()!.UserId, ExerciseType = ExerciseSeedData.ExerciseTypes()[2] },
            new UserExercise { Id = 4,  ExerciseTypeId = 1, Version = 2, CreatedAt = date, UpdatedAt = date,
                UserId = Users().First()!.UserId, ExerciseType = ExerciseSeedData.ExerciseTypes().First() },
            // User exercise for non admin user
            new UserExercise { Id = 5,  ExerciseTypeId = 2, Version = 2, CreatedAt = date, UpdatedAt = date,
                UserId = Users().Last()!.UserId, ExerciseType = ExerciseSeedData.ExerciseTypes()[1] },
        ];
    }
    
    /// <summary>
    ///  SeedData data for WorkoutSession
    ///  It must be the third seed data to be inserted from this category
    ///  It depends on UserExerciseSeedData.UserExercises
    /// </summary>
    /// <returns> List of WorkoutSession</returns>
    public static List<WorkoutSession> WorkoutSessions()
    {
        var firstUser = Users().First()!.UserId; 
        var firstUserExercises = UserExercises().Where(e=> e.UserId == firstUser).ToList();
        
        var lastUser = Users().Last()!.UserId;
        var lastUserExercises = UserExercises().Where(e=> e.UserId == lastUser).ToList();
        
        var date = new DateTime(2022, 1, 1);
        return
        [
            // Workout sessions for admin user
            new WorkoutSession { Id = 1, SortOrder = 1, CreatedAt = date, UpdatedAt = date,
                UserId = firstUser,
                Name = "Test Workout Session 1",
                DayOfWeek = DayOfWeek.Monday,
                UserExercises = firstUserExercises
            },
            new WorkoutSession { Id = 2, SortOrder = 2, CreatedAt = date, UpdatedAt = date,
                UserId = firstUser,
                Name = "Test Workout Session 2",
                DayOfWeek = DayOfWeek.Tuesday,
                UserExercises = firstUserExercises
            },
            new WorkoutSession { Id = 3, SortOrder = 3, CreatedAt = date, UpdatedAt = date,
                UserId = firstUser,
                Name = "Test Workout Session 3",
                DayOfWeek = DayOfWeek.Wednesday,
                UserExercises = firstUserExercises
            },
            // Workout session for non admin user
            new WorkoutSession { Id = 4, SortOrder = 1, CreatedAt = date, UpdatedAt = date,
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
    public static List<UserExerciseHistory> UserExerciseHistories()
    {
        var date = new DateTime(2022, 1, 1);
        return
        [
            // User Exercise Histories for admin user
            new UserExerciseHistory { Id = 1, Repetition = 5, Weight = 100,
                CreatedAt = date, UpdatedAt = date,
                UserExerciseId = UserExercises().First().Id,
                Change = date,
                UserId = Users().First().UserId
            },
            new UserExerciseHistory { Id = 2, Repetition = 5, Weight = 100,
                CreatedAt = date, UpdatedAt = date,
                UserExerciseId = UserExercises()[1].Id,
                Change = date,
                UserId = Users().First().UserId
            },
            new UserExerciseHistory { Id = 3, Repetition = 5, Weight = 100,
                CreatedAt = date, UpdatedAt = date,
                UserExerciseId = UserExercises()[2].Id,
                Change = date,
                UserId = Users().First().UserId
            },
            // User Exercise Histories for non admin user
            new UserExerciseHistory { Id = 4, Repetition = 5, Weight = 100,
                CreatedAt = date, UpdatedAt = date,
                UserExerciseId = UserExercises()[3].Id,
                Change = date,
                UserId = Users().Last().UserId
            },
            
        ];
    }
}
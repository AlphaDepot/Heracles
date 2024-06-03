using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.UserExercises.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for UserExercise entity for unit tests
/// </summary>
public abstract class UserExerciseFixture
{
    public static List<UserExercise> Get() => UserExerciseSeedData.UserExercises();

    public static List<UserExercise> Query(QueryRequestDto? query, string userId, bool isAdmin)
    {
        return Fixtures.QueryWithUser(Get(), query, userId, isAdmin);
        
    }
}
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for Equipment entity for unit tests
/// </summary>
public abstract class WorkoutSessionFixture
{
    public static List<WorkoutSession> Get() => UserExerciseSeedData.WorkoutSessions();

    public static List<WorkoutSession> Query(QueryRequestDto? query, string? userId, bool isAdmin)
    {
      

        return  Fixtures.QueryWithUser(Get(), query, userId, isAdmin);
    }
}
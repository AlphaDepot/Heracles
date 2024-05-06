using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for Equipment entity for unit tests
/// </summary>
public class WorkoutSessionFixture
{
    public static List<WorkoutSession> Get() => UserExerciseSeedData.WorkoutSessions();

    public static List<WorkoutSession> Query(QueryRequest? query, string? userId, bool isAdmin)
    {
      

        return  Fixtures.QueryWithUser(Get(), query, userId, isAdmin);
    }
}
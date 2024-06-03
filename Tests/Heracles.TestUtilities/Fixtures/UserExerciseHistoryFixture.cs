using System.Globalization;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///   Fixture for UserExerciseHistory entity for unit tests
/// </summary>
public abstract class UserExerciseHistoryFixture
{
    public static List<UserExerciseHistory> Get() => UserExerciseSeedData.UserExerciseHistories();
    
    public static List<UserExerciseHistory> Query(QueryRequestDto? query, string userId, bool isAdmin, int? id  = 0)
    {
        return Fixtures.QueryWithUser(Get(), query, userId, isAdmin);
    }
    
}
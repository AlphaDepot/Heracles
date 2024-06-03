using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.ExercisesType.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for Equipment entity for unit tests
/// </summary>
public abstract class ExerciseTypeFixture
{
    /// <summary>
    /// Retrieves a list of exercise types.
    /// </summary>
    /// <returns>A list of exercise types.</returns>
    public static List<ExerciseType> Get() => ExerciseSeedData.ExerciseTypes();

    /// <summary>
    /// Retrieves a list of exercise types based on the specified query.
    /// </summary>
    /// <param name="query">The query request containing search and sort parameters.</param>
    /// <returns>A list of exercise types filtered and sorted according to the query.</returns>
    public static List<ExerciseType> Query(QueryRequestDto? query)
    {
        return Fixtures.Query(Get(), query);
    }
    
}
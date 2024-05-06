using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleFunctions.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for MuscleFunction entity for unit tests
/// </summary>
public class MuscleFunctionFixture
{
    /// <summary>
    /// Retrieves a list of MuscleFunction objects.
    /// </summary>
    /// <returns>A list of MuscleFunction objects.</returns>
    public static List<MuscleFunction> Get() => ExerciseSeedData.MuscleFunctions();

    /// <summary>
    /// Retrieves a list of MuscleFunction entities based on the provided query.
    /// </summary>
    /// <param name="query">The query parameters to filter and sort the MuscleFunction entities.</param>
    /// <returns>A list of MuscleFunction entities that match the query parameters.</returns>
    public static List<MuscleFunction> GetQueryRequest(QueryRequest? query)
    {
        return Fixtures.Query(Get(), query);
    }
}
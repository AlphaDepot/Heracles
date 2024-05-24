using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleGroups.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for MuscleGroups entity for unit tests
/// </summary>
public abstract class MuscleGroupFixture
{
    /// <summary>
    /// Retrieves a list of MuscleGroups.
    /// </summary>
    /// <returns>A list of MuscleGroups.</returns>
    public static List<MuscleGroup> Get() => ExerciseSeedData.MuscleGroups();

    /// <summary>
    /// Gets a list of MuscleGroup entities based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters used to filter and sort the MuscleGroup entities.</param>
    /// <returns>A list of MuscleGroup entities.</returns>
    public static List<MuscleGroup> GetQueryRequest(QueryRequest? query)
    {
        return Fixtures.Query(Get(), query);
    }
}
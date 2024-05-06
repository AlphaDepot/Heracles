using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for ExerciseMuscleGroups entity for unit tests
/// </summary>
public static class ExerciseMuscleGroupFixture
{
    /// <summary>
    /// Retrieves a list of ExerciseMuscleGroup objects.
    /// </summary>
    /// <param name="query">The QueryRequest object containing the search, sorting, and pagination parameters.</param>
    /// <param name="id">The optional ID of the ExerciseMuscleGroup object to filter the results by.</param>
    /// <returns>A list of ExerciseMuscleGroup objects that meet the query criteria.</returns>
    public static List<ExerciseMuscleGroup> Get() => ExerciseSeedData.ExerciseMuscleGroups();

    /// <summary>
    /// Executes a query on the list of ExerciseMuscleGroup objects based on the provided parameters.
    /// </summary>
    /// <param name="query">The QueryRequest object containing the search, sorting, and pagination parameters.</param>
    /// <param name="id">The optional ID of the ExerciseMuscleGroup object to filter the results by.</param>
    /// <returns>A list of ExerciseMuscleGroup objects that meet the query criteria.</returns>
    public static List<ExerciseMuscleGroup> Query(QueryRequest? query, int? id = 0)
    {
        var exerciseMuscleGroups = Get();

        // Filter  by exercise type id if provided
        if (id != 0)
            exerciseMuscleGroups = exerciseMuscleGroups.Where(e => e.ExerciseTypeId == id).ToList();
        
        return Fixtures.Query(exerciseMuscleGroups, query);
    }
    
}
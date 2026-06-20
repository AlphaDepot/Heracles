using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.ExerciseMuscleGroups;

/// <summary>
///     Represents the groupRequest to update an <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <param name="Id"> The ID of the <see cref="ExerciseMuscleGroup" /> to update.</param>
/// <param name="Concurrency"> The concurrency stamp of the <see cref="ExerciseMuscleGroup" /> to update.</param>
/// <param name="FunctionPercentage">
///     The percentage of the <see cref="ExerciseMuscleGroup" /> to associate with the
///     <see cref="ExerciseMuscleGroup" />.
/// </param>
public record UpdateExerciseMuscleGroupRequest(int Id, string? Concurrency, double FunctionPercentage);

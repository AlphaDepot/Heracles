using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.ExerciseTypes;

/// <summary>
///     Removes a <see cref="ExerciseMuscleGroup" /> from an <see cref="ExerciseType" />.
/// </summary>
/// <param name="ExerciseTypeId"> The Id of the <see cref="ExerciseType" />.</param>
/// <param name="MuscleGroupId"> The Id of the <see cref="ExerciseMuscleGroup" />.</param>
public record DetachExerciseMuscleGroupRequest(int ExerciseTypeId, int MuscleGroupId);

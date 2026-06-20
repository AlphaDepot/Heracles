using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.ExerciseMuscleGroups;

/// <summary>
///     Represents the groupRequest to create a new <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <param name="ExerciseTypeId">
///     The ID of the <see cref="ExerciseMuscleGroup" /> to associate with the
///     <see cref="MuscleGroup" />.
/// </param>
/// <param name="MuscleId">
///     The ID of the <see cref="MuscleGroup" /> to associate with the
///     <see cref="ExerciseMuscleGroup" />.
/// </param>
/// <param name="FunctionId">
///     The ID of the <see cref="MuscleFunction" /> to associate with the
///     <see cref="ExerciseMuscleGroup" />.
/// </param>
/// <param name="FunctionPercentage">
///     The percentage of the <see cref="MuscleFunction" /> to associate with the
///     <see cref="ExerciseMuscleGroup" />.
/// </param>
public record CreateExerciseMuscleGroupRequest(
	int ExerciseTypeId,
	int MuscleId,
	int FunctionId,
	double FunctionPercentage);

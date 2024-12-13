using Application.Features.ExerciseMuscleGroups.Commands;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleGroups;

namespace Application.Features.ExerciseMuscleGroups;

/// <summary>
///     <see cref="ExerciseMuscleGroup" /> Extensions
/// </summary>
public static class ExerciseMuscleGroupExtensions
{
	/// <summary>
	///     Map create request to a <see cref="ExerciseMuscleGroup" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateExerciseMuscleGroupRequest" /> request</param>
	/// <param name="muscleGroup"><see cref="MuscleGroup" /> entity</param>
	/// <param name="muscleFunction"><see cref="MuscleFunction" /> entity</param>
	/// <returns><see cref="ExerciseMuscleGroup" /> entity</returns>
	public static ExerciseMuscleGroup MapCreateRequestToDbEntity(
		this CreateExerciseMuscleGroupRequest request, MuscleGroup muscleGroup, MuscleFunction muscleFunction)
	{
		return new ExerciseMuscleGroup
		{
			ExerciseTypeId = request.ExerciseTypeId,
			Muscle = muscleGroup,
			Function = muscleFunction,
			FunctionPercentage = request.FunctionPercentage
		};
	}

	/// <summary>
	///     Map update request to a <see cref="ExerciseMuscleGroup" /> entity
	/// </summary>
	/// <param name="request"><see cref="UpdateExerciseMuscleGroupRequest" /> request</param>
	/// <param name="exerciseMuscleGroup"><see cref="ExerciseMuscleGroup" /> entity</param>
	/// <returns><see cref="ExerciseMuscleGroup" /> entity</returns>
	public static ExerciseMuscleGroup MapUpdateRequestToDbEntity(
		this UpdateExerciseMuscleGroupRequest request, ExerciseMuscleGroup exerciseMuscleGroup)
	{
		return new ExerciseMuscleGroup
		{
			Id = request.Id,
			ExerciseTypeId = exerciseMuscleGroup.ExerciseTypeId,
			Muscle = exerciseMuscleGroup.Muscle,
			Function = exerciseMuscleGroup.Function,
			FunctionPercentage = request.FunctionPercentage,
			CreatedAt = exerciseMuscleGroup.CreatedAt,
			UpdatedAt = exerciseMuscleGroup.UpdatedAt
		};
	}
}

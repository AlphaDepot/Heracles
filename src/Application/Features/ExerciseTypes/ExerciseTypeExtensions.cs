using Application.Features.ExerciseTypes.Commands;

namespace Application.Features.ExerciseTypes;

/// <summary>
///     <see cref="ExerciseType" /> extension methods.
/// </summary>
public static class ExerciseTypeExtensions
{
	/// <summary>
	///     Maps a <see cref="CreateExerciseTypeRequest" /> to a <see cref="ExerciseType" />.
	/// </summary>
	/// <param name="request"><see cref="CreateExerciseTypeRequest" />.</param>
	/// <returns><see cref="ExerciseType" />.</returns>
	public static ExerciseType MapCreateRequestToDbEntity(this CreateExerciseTypeRequest request)
	{
		return new ExerciseType
		{
			Name = request.Name,
			Description = request.Description,
			ImageUrl = request.ImageUrl
		};
	}

	/// <summary>
	///     Maps an <see cref="UpdateExerciseTypeRequest" /> to a <see cref="ExerciseType" />.
	/// </summary>
	/// <param name="request"><see cref="UpdateExerciseTypeRequest" />.</param>
	/// <param name="exerciseType"><see cref="ExerciseType" />.</param>
	/// <returns><see cref="ExerciseType" />.</returns>
	public static ExerciseType MapUpdateRequestToDbEntity(this UpdateExerciseTypeRequest request,
		ExerciseType exerciseType)
	{
		// only map muscle groups if they are provided
		var newExerciseType = new ExerciseType
		{
			Id = request.Id,
			Name = request.Name,
			Description = request.Description,
			ImageUrl = request.ImageUrl,
			CreatedAt = exerciseType.CreatedAt,
			UpdatedAt = exerciseType.UpdatedAt,
			MuscleGroups = exerciseType.MuscleGroups
		};


		return newExerciseType;
	}
}

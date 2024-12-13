using Application.Features.MuscleGroups.Commands;

namespace Application.Features.MuscleGroups;

/// <summary>
///     <see cref="MuscleGroup" /> Extensions
/// </summary>
public static class MuscleGroupExtensions
{
	/// <summary>
	///     Map <see cref="CreateMuscleGroupRequest" /> to a <see cref="MuscleGroup" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateMuscleGroupRequest" /> request</param>
	/// <returns><see cref="MuscleGroup" /> entity</returns>
	public static MuscleGroup MapCreateRequestToDbEntity(this CreateMuscleGroupRequest request)
	{
		return new MuscleGroup
		{
			Name = request.Name
		};
	}

	public static MuscleGroup MapUpdateRequestToDbEntity(this UpdateMuscleGroupRequest request, MuscleGroup muscleGroup)
	{
		return new MuscleGroup
		{
			Id = request.Id,
			Name = request.Name,
			CreatedAt = muscleGroup.CreatedAt,
			UpdatedAt = muscleGroup.UpdatedAt
		};
	}
}

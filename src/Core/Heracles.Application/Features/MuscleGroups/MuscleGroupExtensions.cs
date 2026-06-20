using Heracles.Domain.Entities;
using Heracles.Shared.Requests.MuscleGroups;

namespace Heracles.Application.Features.MuscleGroups;

/// <summary>
///     <see cref="MuscleGroup" /> Extensions
/// </summary>
public static class MuscleGroupExtensions
{
	/// <summary>
	///     Map <see cref="CreateMuscleGroupRequest" /> to a <see cref="MuscleGroup" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateMuscleGroupRequest" /> groupRequest</param>
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

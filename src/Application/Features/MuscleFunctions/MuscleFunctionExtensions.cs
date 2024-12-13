using Application.Features.MuscleFunctions.Commands;

namespace Application.Features.MuscleFunctions;

/// <summary>
///     <see cref="MuscleFunction" /> Extensions
/// </summary>
public static class MuscleFunctionExtensions
{
	/// <summary>
	///     Map <see cref="CreateMuscleFunctionRequest" /> to a <see cref="MuscleFunction" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateMuscleFunctionRequest" /> request</param>
	/// <returns><see cref="MuscleFunction" /> entity</returns>
	public static MuscleFunction MapCreateRequestToDbEntity(this CreateMuscleFunctionRequest request)
	{
		return new MuscleFunction
		{
			Name = request.Name
		};
	}

	/// <summary>
	///     Map <see cref="UpdateMuscleFunctionRequest" /> to a <see cref="MuscleFunction" /> entity
	/// </summary>
	/// <param name="request"><see cref="UpdateMuscleFunctionRequest" /> request</param>
	/// <param name="muscleFunction"><see cref="MuscleFunction" /> entity</param>
	/// <returns><see cref="MuscleFunction" /> entity</returns>
	public static MuscleFunction MapUpdateRequestToDbEntity(this UpdateMuscleFunctionRequest request,
		MuscleFunction muscleFunction)
	{
		return new MuscleFunction
		{
			Id = request.Id,
			Name = request.Name,
			CreatedAt = muscleFunction.CreatedAt,
			UpdatedAt = muscleFunction.UpdatedAt
		};
	}
}

using Application.Features.UserExercises.Commands;

namespace Application.Features.UserExercises;

/// <summary>
///     <see cref="UserExercise" /> Extensions
/// </summary>
public static class UserExerciseExtensions
{
	/// <summary>
	///     Map User create request to a <see cref="UserExercise" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateUserExerciseRequest" /> request</param>
	/// <returns><see cref="UserExercise" /> entity</returns>
	public static UserExercise MapCreateRequestToDbEntity(this CreateUserExerciseRequest request)
	{
		return new UserExercise
		{
			UserId = request.UserId,
			ExerciseTypeId = request.ExerciseTypeId,
			StaticResistance = request.StaticResistance,
			PercentageResistance = request.PercentageResistance,
			CurrentWeight = request.CurrentWeight,
			PersonalRecord = request.PersonalRecord,
			DurationInSeconds = request.DurationInSeconds ?? 0,
			SortOrder = request.SortOrder ?? 0,
			Repetitions = request.Repetitions ?? 0,
			Sets = request.Sets ?? 0,
			Timed = request.Timed ?? false,
			BodyWeight = request.BodyWeight ?? false
		};
	}

	/// <summary>
	///     Map User update request to a <see cref="UserExercise" /> entity
	/// </summary>
	/// <param name="request"><see cref="UpdateUserExerciseRequest" /> request</param>
	/// <param name="userExercise"><see cref="UserExercise" /> entity</param>
	/// <returns><see cref="UserExercise" /> entity</returns>
	public static UserExercise MapUpdateRequestToDbEntity(this UpdateUserExerciseRequest request,
		UserExercise userExercise)
	{
		return new UserExercise
		{
			Id = request.Id,
			UserId = userExercise.UserId,
			ExerciseTypeId = userExercise.ExerciseTypeId,
			StaticResistance = request.StaticResistance,
			PercentageResistance = request.PercentageResistance,
			CurrentWeight = request.CurrentWeight,
			PersonalRecord = request.PersonalRecord,
			DurationInSeconds = request.DurationInSeconds ?? 0,
			SortOrder = request.SortOrder ?? 0,
			Repetitions = request.Repetitions ?? 0,
			Sets = request.Sets ?? 0,
			Timed = request.Timed ?? false,
			BodyWeight = request.BodyWeight ?? false
		};
	}
}

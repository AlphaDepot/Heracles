using Application.Features.UserExerciseHistories.Commands;

namespace Application.Features.UserExerciseHistories;

/// <summary>
///     <see cref="UserExerciseHistory" /> Extensions
/// </summary>
public static class UserExerciseHistoryExtensions
{
	/// <summary>
	///     Maps a <see cref="CreateUserExerciseHistoryRequest" /> to a <see cref="UserExerciseHistory" />.
	/// </summary>
	/// <param name="request"> The <see cref="CreateUserExerciseHistoryRequest" /> to map.</param>
	/// <returns> The mapped <see cref="UserExerciseHistory" />.</returns>
	public static UserExerciseHistory MapCreateRequestToDbEntity(this CreateUserExerciseHistoryRequest request)
	{
		return new UserExerciseHistory
		{
			UserExerciseId = request.UserExerciseId,
			Weight = request.Weight,
			Repetition = request.Repetition,
			UserId = request.UserId
		};
	}

	/// <summary>
	///     Maps a <see cref="UpdateUserExerciseHistoryRequest" /> to a <see cref="UserExerciseHistory" />.
	/// </summary>
	/// <param name="request"> The <see cref="UpdateUserExerciseHistoryRequest" /> to map.</param>
	/// <returns> The mapped <see cref="UserExerciseHistory" />.</returns>
	public static UserExerciseHistory MapUpdateRequestToDbEntity(this UpdateUserExerciseHistoryRequest request)
	{
		return new UserExerciseHistory
		{
			Id = request.Id,
			Weight = request.Weight,
			Repetition = request.Repetition,
			Concurrency = request.Concurrency,
			UserExerciseId = request.UserExerciseId,
			UserId = request.UserId
		};
	}
}

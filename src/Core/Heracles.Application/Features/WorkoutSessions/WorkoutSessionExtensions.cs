using Heracles.Domain.Entities;
using Heracles.Shared.Requests.WorkoutSessions;
using Heracles.Shared.Utilities;

namespace Heracles.Application.Features.WorkoutSessions;

/// <summary>
///     <see cref="WorkoutSession" /> Extensions
/// </summary>
public static class WorkoutSessionExtensions
{
	/// <summary>
	///     Maps a <see cref="CreateWorkoutSessionRequest" /> to a <see cref="WorkoutSession" />.
	/// </summary>
	/// <param name="request">The <see cref="CreateWorkoutSessionRequest" /> to map.</param>
	/// <returns>The mapped <see cref="WorkoutSession" />.</returns>
	public static WorkoutSession MapCreateRequestToDbEntity(this CreateWorkoutSessionRequest request)
	{
		return new WorkoutSession
		{
			UserId = request.UserId,
			Name = request.Name,
			DayOfWeek = DayOfWeekBuilder.GetDayOfWeek(request.DayOfWeek) ?? DayOfWeek.Sunday,
			SortOrder = request.SortOrder
		};
	}

	/// <summary>
	///     Maps a <see cref="UpdateWorkoutSessionRequest" /> to a <see cref="WorkoutSession" />.
	/// </summary>
	/// <param name="request">The <see cref="UpdateWorkoutSessionRequest" /> to map.</param>
	/// <returns>The mapped <see cref="WorkoutSession" />.</returns>
	public static WorkoutSession MapUpdateRequestToDbEntity(this UpdateWorkoutSessionRequest request)
	{
		var dayOfWeek = request.DayOfWeek != null
			? DayOfWeekBuilder.GetDayOfWeek(request.DayOfWeek) ?? DayOfWeek.Sunday
			: DayOfWeek.Sunday;

		return new WorkoutSession
		{
			Id = request.Id,
			UserId = request.UserId,
			Name = request.Name,
			DayOfWeek = dayOfWeek,
			SortOrder = request.SortOrder,
			Concurrency = request.Concurrency
		};
	}
}

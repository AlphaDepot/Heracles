using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.WorkoutSessions;

/// <summary>
///     Represents the groupRequest to update a <see cref="WorkoutSession" />.
/// </summary>
public class UpdateWorkoutSessionRequest
{
	public int Id { get; set; }
	public string? Concurrency { get; set; }
	public required string Name { get; set; }
	public string? DayOfWeek { get; set; }
	public int SortOrder { get; set; }
	public required string UserId { get; set; }
}

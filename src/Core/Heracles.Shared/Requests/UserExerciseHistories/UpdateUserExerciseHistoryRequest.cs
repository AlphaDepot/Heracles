using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.UserExerciseHistories;

/// <summary>
///     Represents the groupRequest to update a <see cref="UserExerciseHistory" />.
/// </summary>
public class UpdateUserExerciseHistoryRequest
{
	public int Id { get; set; }
	public required string Concurrency { get; set; }
	public int UserExerciseId { get; set; }
	public string UserId { get; set; } = null!;
	public double Weight { get; set; }
	public int Repetition { get; set; }
}

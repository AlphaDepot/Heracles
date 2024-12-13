using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using Application.Features.UserExercises;

namespace Application.Features.WorkoutSessions;

/// <summary>
///     Model for Workout Session entity
/// </summary>
public sealed class WorkoutSession : IEntity, IUserEntity, IHasName
{
	/// <summary>
	///     Day of the week the workout session is scheduled for
	/// </summary>
	public DayOfWeek DayOfWeek { get; set; }

	/// <summary>
	///     Order of the workout session in the week
	/// </summary>
	public int? SortOrder { get; set; }

	/// List of
	/// <see cref="UserExercise" />
	public List<UserExercise>? UserExercises { get; set; }

	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }
	[StringLength(255)] public string Name { get; set; } = null!;
	[StringLength(50)] public required string UserId { get; set; }
}

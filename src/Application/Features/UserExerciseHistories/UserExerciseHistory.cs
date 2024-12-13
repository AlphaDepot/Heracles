using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Common.Interfaces;

namespace Application.Features.UserExerciseHistories;

/// <summary>
///     Model for User Exercise History entity
/// </summary>
public sealed class UserExerciseHistory : IEntity, IUserEntity
{
	[ForeignKey("UserExercise")] public required int UserExerciseId { get; set; }
	public double Weight { get; set; }
	public int Repetition { get; set; }
	public DateTime Change { get; set; } = DateTime.UtcNow;
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }
	[StringLength(50)] public required string UserId { get; set; }
}

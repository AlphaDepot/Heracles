using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;

namespace Application.Features.MuscleFunctions;

/// <summary>
///     Muscle Function Entity
/// </summary>
public sealed class MuscleFunction : IEntity, IHasName
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }
	[StringLength(50)] public required string Name { get; set; }
}

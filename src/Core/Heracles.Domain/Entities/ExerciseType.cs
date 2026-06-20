using System.ComponentModel.DataAnnotations;
using Heracles.Domain.Interfaces;

namespace Heracles.Domain.Entities;

/// <summary>
///     Represents a type of exercise that can be performed.
/// </summary>
public class ExerciseType : IEntity, IHasName
{
	/// <summary>
	///     Description of the exercise type and how to perform it.
	///     it can be up to 1000 characters long.
	/// </summary>
	[StringLength(1000)]
	public string? Description { get; set; }

	/// <summary>
	///     Images of the exercise type.
	/// </summary>
	public List<string>? Images { get; set; }

	/// <summary>
	///     List of <see cref="ExerciseMuscleGroup" />
	/// </summary>
	public List<ExerciseMuscleGroup>? MuscleGroups { get; set; }

	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }

	[StringLength(255)] public required string Name { get; set; }
}

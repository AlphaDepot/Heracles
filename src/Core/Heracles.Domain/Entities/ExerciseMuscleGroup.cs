using System.ComponentModel.DataAnnotations;
using Heracles.Domain.Interfaces;

namespace Heracles.Domain.Entities;

/// <summary>
///     Exercise Muscle Group entity
/// </summary>
public sealed class ExerciseMuscleGroup : IEntity
{
	public int ExerciseTypeId { get; set; }

	/// <see cref="MuscleGroup" />
	public required MuscleGroup Muscle { get; set; }

	public int MuscleId { get; set; }

	/// <see cref="MuscleFunction" />
	public required MuscleFunction Function { get; set; }

	public int FunctionId { get; set; }

	/// <summary>
	///     Percentage of muscle used in the exercise (e.g., 100% for the Brachialis, 50% for a Bicep, etc...)
	/// </summary>
	public double FunctionPercentage { get; set; }

	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }
}

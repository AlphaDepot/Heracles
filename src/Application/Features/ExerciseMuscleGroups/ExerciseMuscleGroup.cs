using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleGroups;

namespace Application.Features.ExerciseMuscleGroups;

/// <summary>
///     Exercise Muscle Group entity
/// </summary>
public sealed class ExerciseMuscleGroup : IEntity
{
	public int ExerciseTypeId { get; set; }

	/// <see cref="MuscleGroup" />
	public required MuscleGroup Muscle { get; set; }

	/// <see cref="MuscleFunction" />
	public required MuscleFunction Function { get; set; }

	/// <summary>
	///     Percentage of muscle used in the exercise (e.g., 100% for the Brachialis, 50% for a Bicep, etc...)
	/// </summary>
	public double FunctionPercentage { get; set; }

	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }
}

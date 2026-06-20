using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.UserExercises;

/// <summary>
///     Represents the groupRequest to update a <see cref="UserExercise" />.
/// </summary>
public class UpdateUserExerciseRequest
{
	public int Id { get; set; }
	public required string Concurrency { get; set; }
	public double? StaticResistance { get; set; }
	public double? PercentageResistance { get; set; }
	public double? CurrentWeight { get; set; }
	public double? PersonalRecord { get; set; }
	public int? DurationInSeconds { get; set; }
	public int? SortOrder { get; set; }
	public int? Repetitions { get; set; }
	public int? Sets { get; set; }
	public bool? Timed { get; set; }
	public bool? BodyWeight { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using Application.Features.EquipmentGroups;
using Application.Features.ExerciseTypes;
using Application.Features.UserExerciseHistories;
using Application.Features.WorkoutSessions;

namespace Application.Features.UserExercises;

/// <summary>
///     Model for User Exercise entity
/// </summary>
public sealed class UserExercise : IEntity, IUserEntity
{
	[ForeignKey("ExerciseDetail")] public int ExerciseTypeId { get; set; }

	/// <see cref="ExerciseType" />
	public ExerciseType ExerciseType { get; set; } = null!;

	/// <summary>
	///     Version of the user exercise, used to have more than one record for the same exercise
	/// </summary>
	public int Version { get; set; } = 1;

	/// <summary>
	///     Resistance in lbs or kg, such as in a resistance band or cable machine where the cable and pulley add resistance.
	/// </summary>
	public double? StaticResistance { get; set; }

	/// <summary>
	///     Lift resistance as a percentage multiplier, Such as +50%  or -50% of the weight being lifted.
	///     Useful in cable machines that increase resistance artificially to increase the range of weight available.
	/// </summary>
	public double? PercentageResistance { get; set; }

	/// <summary>
	///     Current weight being lifted in lbs or kg
	/// </summary>
	public double? CurrentWeight { get; set; }

	/// <summary>
	///     Personal record for the exercise in lbs or kg
	/// </summary>
	public double? PersonalRecord { get; set; }

	/// <summary>
	///     Duration of the exercise in seconds,
	///     Used for timed exercises like planks, wall sits, etc.
	/// </summary>
	public int DurationInSeconds { get; set; }

	/// <summary>
	///     Order of the exercise in the workout
	/// </summary>
	public int SortOrder { get; set; }

	/// <summary>
	///     Number of repetitions for the exercise per set
	/// </summary>
	public int Repetitions { get; set; }

	/// <summary>
	///     Number of sets for the exercise
	/// </summary>
	public int Sets { get; set; }

	/// <summary>
	///     Sets  if the exercise is timed or not
	/// </summary>
	public bool Timed { get; set; }

	/// <summary>
	///     If the exercise is body weight only
	/// </summary>
	public bool BodyWeight { get; set; }

	/// List of
	/// <see cref="UserExerciseHistory" />
	public IEnumerable<UserExerciseHistory>? ExerciseHistories { get; set; }

	/// List of
	/// <see cref="UserExerciseHistory" />
	[JsonIgnore]
	public IEnumerable<WorkoutSession>? WorkoutSessions { get; set; }

	/// <see cref="EquipmentGroup" />
	public EquipmentGroup? EquipmentGroup { get; set; }

	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }
	[StringLength(50)] public required string UserId { get; set; }
}

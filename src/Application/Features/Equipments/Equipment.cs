using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;

namespace Application.Features.Equipments;

/// <summary>
///     Equipment entity
/// </summary>
public sealed class Equipment : IEntity, IHasType
{
	/// <summary>
	///     Equipment Weight (e.g. 10, 15, 20, 25, 30, 35, 40, 45, 50) in lbs or kg
	///     depending on the front end desired configuration
	/// </summary>
	public double Weight { get; set; }

	/// <summary>
	///     Equipment resistance in lbs or kg, such as in a resistance band or cable machine where the cable and pulley add
	///     resistance.
	/// </summary>
	public double Resistance { get; set; }

	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }
	[StringLength(255)] public required string Type { get; set; }
}

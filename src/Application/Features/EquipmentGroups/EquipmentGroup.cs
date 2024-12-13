using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using Application.Features.Equipments;

namespace Application.Features.EquipmentGroups;

/// <summary>
///     Equipment Group
/// </summary>
public class EquipmentGroup : IEntity, IHasName
{
	/// <summary>
	///     List of <see cref="Equipment" />
	/// </summary>
	public List<Equipment>? Equipments { get; set; }

	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	[StringLength(36)] public string? Concurrency { get; set; }

	[StringLength(255)] public required string Name { get; set; }
}

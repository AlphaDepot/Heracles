using System.ComponentModel.DataAnnotations;
using Heracles.Domain.Interfaces;

namespace Heracles.Domain.Entities;

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

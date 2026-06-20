using Heracles.Domain.Entities;
using Heracles.Shared.Requests.EquipmentGroups;

namespace Heracles.Application.Features.EquipmentGroups;

/// <summary>
///     Equipment Group Extensions
/// </summary>
public static class EquipmentGroupExtensions
{
	/// <summary>
	///     Map Create groupRequest to a <see cref="EquipmentGroup" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateEquipmentGroupRequest" /> groupRequest</param>
	/// <returns><see cref="EquipmentGroup" /> entity</returns>
	public static EquipmentGroup MapCreateRequestToDbEntity(this CreateEquipmentGroupRequest request)
	{
		return new EquipmentGroup
		{
			Name = request.Name
		};
	}

	/// <summary>
	///     Map Update groupRequest to a <see cref="EquipmentGroup" /> entity
	/// </summary>
	/// <param name="request"><see cref="UpdateEquipmentGroupRequest" /> groupRequest</param>
	/// <param name="equipmentGroup"><see cref="EquipmentGroup" /> entity</param>
	/// <returns><see cref="EquipmentGroup" /> entity</returns>
	public static EquipmentGroup MapUpdateRequestToDbEntity(
		this UpdateEquipmentGroupRequest request, EquipmentGroup equipmentGroup)
	{
		return new EquipmentGroup
		{
			Id = request.Id,
			Name = request.Name,
			CreatedAt = equipmentGroup.CreatedAt,
			UpdatedAt = equipmentGroup.UpdatedAt
		};
	}
}

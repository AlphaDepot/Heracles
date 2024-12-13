using Application.Features.EquipmentGroups.Commands;

namespace Application.Features.EquipmentGroups;

/// <summary>
///     Equipment Group Extensions
/// </summary>
public static class EquipmentGroupExtensions
{
	/// <summary>
	///     Map Create request to a <see cref="EquipmentGroup" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateEquipmentGroupRequest" /> request</param>
	/// <returns><see cref="EquipmentGroup" /> entity</returns>
	public static EquipmentGroup MapCreateRequestToDbEntity(this CreateEquipmentGroupRequest request)
	{
		return new EquipmentGroup
		{
			Name = request.Name
		};
	}

	/// <summary>
	///     Map Update request to a <see cref="EquipmentGroup" /> entity
	/// </summary>
	/// <param name="request"><see cref="UpdateEquipmentGroupRequest" /> request</param>
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

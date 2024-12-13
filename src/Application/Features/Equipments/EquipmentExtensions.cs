using Application.Features.Equipments.Commands;

namespace Application.Features.Equipments;

/// <summary>
///     <see cref="Equipment" /> Extensions
/// </summary>
public static class EquipmentExtensions
{
	/// <summary>
	///     Map Create request to a <see cref="Equipment" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateEquipmentRequest" /> request</param>
	/// <returns><see cref="Equipment" /> entity</returns>
	public static Equipment MapCreateRequestToDbEntity(this CreateEquipmentRequest request)
	{
		return new Equipment
		{
			Type = request.Type,
			Weight = request.Weight,
			Resistance = request.Resistance
		};
	}

	/// <summary>
	///     Map Update request to a <see cref="Equipment" /> entity
	/// </summary>
	/// <param name="request"><see cref="UpdateEquipmentRequest" /> request</param>
	/// <param name="equipment"><see cref="Equipment" /> entity</param>
	/// <returns><see cref="Equipment" /> entity</returns>
	public static Equipment MapUpdateRequestToDbEntity(this UpdateEquipmentRequest request, Equipment equipment)
	{
		return new Equipment
		{
			Id = request.Id,
			Type = request.Type,
			Weight = request.Weight,
			Resistance = request.Resistance,
			CreatedAt = equipment.CreatedAt,
			UpdatedAt = equipment.UpdatedAt
		};
	}
}

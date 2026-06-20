using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.EquipmentGroups;

/// <summary>
///     Represents the request to attach an <see cref="Equipment" /> to an <see cref="EquipmentGroup" />.
/// </summary>
/// <param name="EquipmentGroupId"></param>
/// <param name="EquipmentId"></param>
public record AttachEquipmentGroupRequest(int EquipmentGroupId, int EquipmentId);

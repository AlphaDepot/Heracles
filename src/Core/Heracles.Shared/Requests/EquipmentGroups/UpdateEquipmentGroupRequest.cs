namespace Heracles.Shared.Requests.EquipmentGroups;

public record UpdateEquipmentGroupRequest(int Id, string Name, string? Concurrency);

namespace Heracles.Domain.EquipmentGroups.DTOs;

public class AddEquipmentGroupDto
{
    public int EquipmentGroupId { get; set; }
    public int EquipmentId { get; set; }
}

public class RemoveEquipmentGroupDto
{
    public int EquipmentGroupId { get; set; }
    public int EquipmentId { get; set; }
}
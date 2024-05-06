using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;

namespace Heracles.Domain.EquipmentGroups.Interfaces;

public interface IEquipmentGroupRepository : IGenericRepository<EquipmentGroup>
{
    Task<bool> IsUnique(string name);
}
using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.Equipments.Models;

namespace Heracles.Domain.Equipments.Interfaces;

public interface IEquipmentRepository : IGenericRepository<Equipment>
{
    Task<bool> IsTypeUnique(string type);
}
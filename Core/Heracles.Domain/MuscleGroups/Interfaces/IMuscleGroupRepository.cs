using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.MuscleGroups.Models;

namespace Heracles.Domain.MuscleGroups.Interfaces;

public interface IMuscleGroupRepository : IGenericRepository<MuscleGroup>
{
    Task<bool> IsNameUnique(string name);
}
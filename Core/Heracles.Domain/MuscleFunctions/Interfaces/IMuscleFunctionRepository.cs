using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.MuscleFunctions.Models;

namespace Heracles.Domain.MuscleFunctions.Interfaces;

public interface IMuscleFunctionRepository : IGenericRepository<MuscleFunction>
{
    Task<bool> IsNameUnique(string name);
}
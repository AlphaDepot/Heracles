using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.Equipments.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class EquipmentRepository : GenericRepository<Equipment>, IEquipmentRepository
{
    public EquipmentRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsTypeUnique(string type)
    {
        // returns true if type is unique therefore reverse the result with !
        return !await DbContext.Equipments
            .AnyAsync(e => e.Type.ToLower() == type.ToLower());
    }
}
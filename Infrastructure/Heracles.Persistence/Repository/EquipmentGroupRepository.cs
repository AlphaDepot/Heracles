using Heracles.Domain.EquipmentGroups;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class EquipmentGroupRepository : GenericRepository<EquipmentGroup>, IEquipmentGroupRepository
{
    public EquipmentGroupRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsUnique(string name)
    {
        return !await DbContext.EquipmentGroups
            .AnyAsync(e => e.Name.ToLower() == name.ToLower());
    }
}
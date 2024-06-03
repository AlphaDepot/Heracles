using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Entities;
using Heracles.Domain.Equipments;
using Heracles.Domain.Equipments.Models;

namespace Heracles.Domain.EquipmentGroups.Models;

public class EquipmentGroup : BaseEntity
{
    [StringLength(255)]
    public required string Name { get; set; }
    public List<Equipment>? Equipments { get; set; }
    
    public static Dictionary<string, Expression<Func<EquipmentGroup, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<EquipmentGroup, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "name", e => e.Name }
        };
    }
    
    public static Expression<Func<EquipmentGroup, bool>> GetFilterExpression(string? searchTerm)
    {
        return e => string.IsNullOrEmpty(searchTerm) || e.Name.Contains(searchTerm);
    }
    
}
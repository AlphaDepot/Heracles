using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;

namespace Heracles.Domain.Equipments.Models;

public class Equipment : BaseEntity
{
    // The type of equipment (e.g. Barbell, Dumbbell, Cable, etc.)
    [StringLength(255)] 
    public required string Type { get; set; }

    public double Weight { get; set; }
    public double Resistance { get; set; }
    
    public new static Dictionary<string, Expression<Func<Equipment, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<Equipment, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "type", e => e.Type },
            { "weight", e => e.Weight },
            { "resistance", e => e.Resistance }
        };
    }
    
    public new static Expression<Func<Equipment, bool>> GetFilterExpression(string? searchTerm)
    {
        // search by type weight or resistance
        return e => string.IsNullOrEmpty(searchTerm) || e.Type.Contains(searchTerm) 
                    || e.Weight.ToString().Contains(searchTerm) 
                    || e.Resistance.ToString().Contains(searchTerm);
    }
}
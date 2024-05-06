using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;

namespace Heracles.Domain.MuscleGroups.Models;

public sealed class MuscleGroup : BaseEntity
{
    [StringLength(100)]
    public required string Name { get; set; }
    
    public static Dictionary<string, Expression<Func<MuscleGroup, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<MuscleGroup, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "name", e => e.Name }
        };
    }
    
    public static Expression<Func<MuscleGroup, bool>> GetFilterExpression(string? searchTerm)
    {
        return e => string.IsNullOrEmpty(searchTerm) || e.Name.Contains(searchTerm);
    }
}
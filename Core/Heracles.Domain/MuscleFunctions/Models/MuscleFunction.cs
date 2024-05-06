using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;

namespace Heracles.Domain.MuscleFunctions.Models;

public sealed class MuscleFunction : BaseEntity
{
    [StringLength(100)]
    public required string Name { get; set; }
    
    public static Dictionary<string, Expression<Func<MuscleFunction, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<MuscleFunction, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "name", e => e.Name }
        };
    }
    
    public static Expression<Func<MuscleFunction, bool>> GetFilterExpression(string? searchTerm)
    {
        return e => string.IsNullOrEmpty(searchTerm) || e.Name.Contains(searchTerm);
    }
}
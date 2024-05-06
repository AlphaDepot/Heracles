using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Heracles.Domain.Abstractions.Queries;

public abstract class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid Concurrency { get; set; }
    
    
    public static Dictionary<string, Expression<Func<BaseEntity, object>>> GetSortExpression()
    {
        return  new Dictionary<string, Expression<Func<BaseEntity, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
        };
    }
    
    public static Expression<Func<BaseEntity, bool>> GetFilterExpression(string? searchTerm, string userId,bool isAdmin = false)
    {
        return e => string.IsNullOrEmpty(searchTerm) || e.Id.ToString().Contains(searchTerm);
    }
}
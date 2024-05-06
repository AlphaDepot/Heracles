using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExercises.Models;

namespace Heracles.Domain.WorkoutSessions.Models;


public sealed class WorkoutSession : BaseEntity
{
    
    public required string UserId { get; set; }
    [StringLength(255)]
    public string Name { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; } 
    public int? SortOrder { get; set; }
    
    public List<UserExercise>? UserExercises { get; set; }


    public static Dictionary<string, Expression<Func<WorkoutSession, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<WorkoutSession, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "userId", e => e.UserId },
            { "name", e => e.Name },
            { "dayOfWeek", e => e.DayOfWeek },
            { "sortOrder", e => e.SortOrder },
        };
        
    }
    
    public static Expression<Func<WorkoutSession, bool>> GetFilterExpression(string? searchTerm, string userId,bool isAdmin = false)
    {
        // Initialize filter as null
        Expression<Func<WorkoutSession, bool>>? filter = null;
        
        // if admin and search term is not empty
        if (isAdmin && !string.IsNullOrEmpty(searchTerm))
        {
            filter = x => x.Name.Contains(searchTerm);
        }

        // if admin and search term is empty
        if (isAdmin && string.IsNullOrEmpty(searchTerm))
        {
            filter = null;
        }
        
        // if not admin and search term is not empty
        if (!isAdmin && !string.IsNullOrEmpty(searchTerm))
        {
            filter = x => x.UserId == userId 
                          && x.Name.Contains(searchTerm);
        }
        
        // if not admin and search term is empty
        if (!isAdmin && string.IsNullOrEmpty(searchTerm))
        {
            filter = x => x.UserId == userId;
        }
        
        return filter;
    }
}
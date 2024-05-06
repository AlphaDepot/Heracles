using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;

namespace Heracles.Domain.UserExerciseHistories.Models;


public sealed class UserExerciseHistory  : BaseEntity
{
    public required string UserId { get; set; }
    
    [ForeignKey("UserExercise")]
    public required int UserExerciseId { get; set; }
    public double Weight { get; set; }
    public int Repetition { get; set; }
    public DateTime Change { get; set; } = DateTime.UtcNow;
    
    
    public static Dictionary<string, Expression<Func<UserExerciseHistory, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<UserExerciseHistory, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "userExerciseId", e => e.UserExerciseId },
            { "weight", e => e.Weight },
            { "repetition", e => e.Repetition },
            { "change", e => e.Change },
        };
    }
    
    public static Expression<Func<UserExerciseHistory, bool>> GetFilterExpression(string? searchTerm, string userId,bool isAdmin = false)
    {
        // Initialize filter as null
        Expression<Func<UserExerciseHistory, bool>>? filter = null;
        
        // if admin and search term is not empty
        if (isAdmin && !string.IsNullOrEmpty(searchTerm))
        {
            filter = e => e.UserExerciseId.ToString().Contains(searchTerm) 
                          || e.Weight.ToString().Contains(searchTerm) 
                          || e.Repetition.ToString().Contains(searchTerm) 
                          || e.Change.ToString().Contains(searchTerm);
        }
        
        // if admin and search term is empty
        if (isAdmin && string.IsNullOrEmpty(searchTerm))
        {
            filter = null;
        }
        
        // if not admin and search term is not empty
        if (!isAdmin && !string.IsNullOrEmpty(searchTerm))
        {
            filter = e => e.UserId == userId 
                          && ( e.UserExerciseId.ToString().Contains(searchTerm) 
                               || e.Weight.ToString().Contains(searchTerm) 
                               || e.Repetition.ToString().Contains(searchTerm) 
                               || e.Change.ToString().Contains(searchTerm));
        }
        
        // if not admin and search term is empty
        if (!isAdmin && string.IsNullOrEmpty(searchTerm))
        {
            filter = e => e.UserId == userId;
        }
        
        return filter;
    }
    

    
}
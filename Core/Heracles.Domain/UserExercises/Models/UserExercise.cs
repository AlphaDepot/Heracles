using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.Domain.ExercisesType.Models;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Domain.WorkoutSessions.Models;

namespace Heracles.Domain.UserExercises.Models;

public sealed class UserExercise : BaseEntity
{
    public required string UserId { get; set; } 

    [ForeignKey("ExerciseDetail")] public int ExerciseTypeId { get; set; }

    public ExerciseType ExerciseType { get; set; } = null!;

    // this controls the version of the exercise in the userExercise
    // This is used in case you want to have the same exercise with different data
    public int Version { get; set; } = 1;

    public double? CurrentWeight { get; set; }
    public double? PersonalRecord { get; set; }

    public int DurationInSeconds { get; set; }
    public int SortOrder { get; set; }
    public int Repetitions { get; set; }
    public int Sets { get; set; }

    public bool Timed { get; set; }
    public bool BodyWeight { get; set; }

    public List<UserExerciseHistory>? ExerciseHistories { get; set; }
    public List<WorkoutSession>? WorkoutSessions { get; set; }
    public EquipmentGroup? EquipmentGroup { get; set; }



    public static Dictionary<string, Expression<Func<UserExercise, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<UserExercise, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "currentWeight", e => e.CurrentWeight },
            { "personalRecord", e => e.PersonalRecord },
            { "userId", e => e.UserId },
            { "name", e => e.ExerciseType.Name },
            { "sortOrder", e => e.SortOrder },
            { "durationInSeconds", e => e.DurationInSeconds },
            
        };
        
    }
    
    public static Expression<Func<UserExercise, bool>> GetFilterExpression(string? searchTerm, string userId,bool isAdmin = false)
    {
        // Initialize filter as null
        Expression<Func<UserExercise, bool>>? filter = null;
        
        // if admin and search term is not empty
        if (isAdmin && !string.IsNullOrEmpty(searchTerm))
        {
            filter = e => e.ExerciseType.Name.Contains(searchTerm)
                          || e.CurrentWeight.ToString().Contains(searchTerm) 
                          || e.PersonalRecord.ToString().Contains(searchTerm);
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
                          && (e.ExerciseType.Name.Contains(searchTerm)
                              || e.CurrentWeight.ToString().Contains(searchTerm) 
                              || e.PersonalRecord.ToString().Contains(searchTerm));
        }
        
        // if not admin and search term is empty
        if (!isAdmin && string.IsNullOrEmpty(searchTerm))
        {
            filter = e => e.UserId == userId;
        }
        
        return filter;
    }
}
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Heracles.Domain.Abstractions.Entities;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.Domain.UserExercises.Models;

namespace Heracles.Domain.ExercisesType.Models;
public class ExerciseType : BaseEntity
{
    [StringLength(255)]
    public required string  Name { get; set; }
    [StringLength(1000)]
    public string? Description { get; set; }
    [StringLength(255)]
    public string? ImageUrl { get; set; }
    public List<ExerciseMuscleGroup>? MuscleGroups { get; set; }
    
    [JsonIgnore]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public List<UserExercise>? UserExercises { get; set; }
    
    
    public static Dictionary<string, Expression<Func<ExerciseType, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<ExerciseType, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "name", e => e.Name }
        };
    }
    
    public static Expression<Func<ExerciseType, bool>> GetFilterExpression(string? searchTerm)
    {
        return e => string.IsNullOrEmpty(searchTerm) || e.Name.Contains(searchTerm);
    }
    
}
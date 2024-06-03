using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Entities;
using Heracles.Domain.MuscleFunctions.Models;
using Heracles.Domain.MuscleGroups.Models;

namespace Heracles.Domain.ExerciseMuscleGroups.Models;
public sealed class ExerciseMuscleGroup : BaseEntity
{
    
    public int ExerciseTypeId { get; set; }
    public required MuscleGroup Muscle { get; set; }
    public required MuscleFunction Function { get; set; }
    public double FunctionPercentage { get; set; }

    /// <summary>
    /// Get the sort expressions for ExerciseMuscleGroup.
    /// </summary>
    /// <returns>A dictionary of sort expressions for ExerciseMuscleGroup. The key represents the sort field and the value represents the sort expression.</returns>
    public new static Dictionary<string, Expression<Func<ExerciseMuscleGroup, object>>> GetSortExpression()
    {
        return  new Dictionary<string, Expression<Func<ExerciseMuscleGroup, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "exercise", e => e.ExerciseTypeId },
            { "muscle", e => e.Muscle.Name },
            { "function", e => e.Function.Name },
            { "percentage", e => e.FunctionPercentage }
        };
    }

    /// <summary>
    /// Get filter expression for ExerciseMuscleGroup based on searchTerm.
    /// </summary>
    /// <param name="searchTerm">The search term to filter ExerciseMuscleGroup by.</param>
    /// <returns>The filter expression for ExerciseMuscleGroup.</returns>
    public new static Expression<Func<ExerciseMuscleGroup, bool>> GetFilterExpression(string? searchTerm)
    {
        return e => string.IsNullOrEmpty(searchTerm) || e.Muscle.Name.Contains(searchTerm);
    }
}
namespace Heracles.Domain.ExerciseMuscleGroups.DTOs;

public sealed class CreateExerciseMuscleGroupDto
{
    public int ExerciseTypeId { get; set; }
    public int MuscleGroupId { get; set; }
    public int MuscleFunctionId { get; set; } 
    public double FunctionPercentage { get; set; } 
    
}

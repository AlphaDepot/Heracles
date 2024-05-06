namespace Heracles.Domain.UserExercises.DTOs;

public class CreateUserExerciseDto
{
    public required string UserId { get; set; } 
    public required int ExerciseId { get; set; }
}
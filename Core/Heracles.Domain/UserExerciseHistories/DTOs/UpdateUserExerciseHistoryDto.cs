namespace Heracles.Domain.UserExerciseHistories.DTOs;

public class UpdateUserExerciseHistoryDto
{
    public int Id { get; set; }
    public double Weight { get; set; }
    public int Repetition { get; set; }
    
    public required string UserId { get; set; }
}
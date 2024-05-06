namespace Heracles.Domain.WorkoutSessions.DTOs;

public class WorkoutSessionExerciseDto
{
    public required string UserId { get; set; }
    public int WorkoutSessionId { get; set; }
    public int UserExerciseId { get; set; }
    
}
namespace Heracles.Domain.UserExercises.DTOs;

public class UpdateUserExerciseDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public double? CurrentWeight { get; set; }
    public double? PersonalRecord { get; set; }

    public int? DurationInSeconds { get; set; }
    public int? SortOrder { get; set; }
    public int? Repetitions { get; set; }
    public int? Sets { get; set; }

    public bool? Timed { get; set; }
    public bool? BodyWeight { get; set; }
    
    public int EquipmentGroupId { get; set; }
}
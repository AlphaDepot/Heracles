using FluentValidation;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;

namespace Heracles.Application.Features.ExerciseMuscleGroups.Validators;

/// <summary>
/// Validator for UpdateExerciseMuscleGroupDto objects.
/// </summary>
public class UpdateExerciseMuscleGroupDtoValidator : AbstractValidator<UpdateExerciseMuscleGroupDto>
{
    private readonly IExerciseMuscleGroupRepository _exerciseMuscleGroupRepository;

    public UpdateExerciseMuscleGroupDtoValidator(
        IExerciseMuscleGroupRepository exerciseMuscleGroupRepository)

    {
        _exerciseMuscleGroupRepository = exerciseMuscleGroupRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(ExerciseMuscleGroupExists).WithMessage("ExerciseMuscleGroups with id {PropertyValue} not found");
        
        
        RuleFor(x => x.FunctionPercentage).NotEmpty()
            .WithMessage("FunctionPercentage is required").GreaterThan(0)
            .WithMessage("FunctionPercentage must be greater than 0").LessThanOrEqualTo(100)
            .WithMessage("FunctionPercentage must be less than or equal to 100");
    }

    /// <summary>
    /// Check if an exercise muscle group exists.
    /// </summary>
    /// <param name="id">The id of the exercise muscle group to check.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>True if the exercise muscle group exists, otherwise false.</returns>
    private Task<bool> ExerciseMuscleGroupExists(int id, CancellationToken token)
    {
        return _exerciseMuscleGroupRepository.ItExist(id);
    }
    
}
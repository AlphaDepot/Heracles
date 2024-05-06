using FluentValidation;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleGroups.Interfaces;

namespace Heracles.Application.Features.ExerciseMuscleGroups.Validators;

/// <summary>
/// Validates the CreateExerciseMuscleGroupDto object.
/// </summary>
public class CreateExerciseMuscleGroupDtoValidator : AbstractValidator<CreateExerciseMuscleGroupDto>
{
    private readonly IExerciseMuscleGroupRepository _exerciseMuscleGroupRepository;
    private readonly IMuscleGroupRepository _muscleGroupRepository;
    private readonly IMuscleFunctionRepository _muscleFunctionRepository;
    private readonly IExerciseTypeRepository _exerciseTypeRepository;

    public CreateExerciseMuscleGroupDtoValidator(
        IExerciseMuscleGroupRepository exerciseMuscleGroupRepository,
        IMuscleGroupRepository muscleGroupRepository,
        IMuscleFunctionRepository muscleFunctionRepository,
        IExerciseTypeRepository exerciseTypeRepository
        )
    {
        _exerciseMuscleGroupRepository = exerciseMuscleGroupRepository;
        _muscleGroupRepository = muscleGroupRepository;
        _muscleFunctionRepository = muscleFunctionRepository;
        _exerciseTypeRepository = exerciseTypeRepository;
        
        RuleFor(x => x.ExerciseTypeId)
            .NotEmpty().WithMessage("ExerciseDetailsId is required")
            .MustAsync(ExerciseTypeExists).WithMessage("ExerciseType with id {PropertyValue} not found");
        
        RuleFor(x => x.MuscleGroupId)
            .NotEmpty().WithMessage("MuscleGroupId is required")
            .MustAsync(MuscleGroupExists).WithMessage("MuscleGroup with id {PropertyValue} not found");
        
        RuleFor(x => x.MuscleFunctionId)
            .NotEmpty().WithMessage("MuscleFunctionId is required")
            .MustAsync(MuscleFunctionExists).WithMessage("MuscleFunction with id {PropertyValue} not found");
        
        RuleFor(x => x.FunctionPercentage).NotEmpty()
            .WithMessage("FunctionPercentage is required").GreaterThan(0)
            .WithMessage("FunctionPercentage must be greater than 0").LessThanOrEqualTo(100)
            .WithMessage("FunctionPercentage must be less than or equal to 100");
        
        RuleFor(x => x) 
            .MustAsync(ExerciseMuscleGroupUnique).WithMessage("ExerciseMuscleGroups already exists");
    }

    /// <summary>
    /// Checks if an exercise type exists with the given ID.
    /// </summary>
    /// <param name="id">The ID of the exercise type to check.</param>
    /// <param name="token">A cancellation token for the operation.</param>
    /// <returns>Returns true if the exercise type exists, otherwise false.</returns>
    private Task<bool> ExerciseTypeExists(int id, CancellationToken token)
    {
        return _exerciseTypeRepository.ItExist(id);
    }

    /// <summary>
    /// Checks if a muscle group exists with the given ID.
    /// </summary>
    /// <param name="id">The ID of the muscle group to check.</param>
    /// <param name="token">A cancellation token for the operation.</param>
    /// <returns>Returns true if the muscle group exists, otherwise false.</returns>
    private Task<bool> MuscleGroupExists(int id, CancellationToken token)
    {
        return _muscleGroupRepository.ItExist(id);
    }

    /// <summary>
    /// Checks if a muscle function exists with the given ID.
    /// </summary>
    /// <param name="id">The ID of the muscle function to check.</param>
    /// <param name="token">A cancellation token for the operation.</param>
    /// <returns>Returns true if the muscle function exists, otherwise false.</returns>
    private Task<bool> MuscleFunctionExists(int id, CancellationToken token)
    {
        return _muscleFunctionRepository.ItExist(id);
    }

    /// <summary>
    /// Checks if an exercise muscle group is unique.
    /// </summary>
    /// <param name="dto">The CreateExerciseMuscleGroupDto containing the exercise type, muscle group, muscle function, and function percentage.</param>
    /// <param name="token">A cancellation token for the operation.</param>
    /// <returns>Returns true if the exercise muscle group is unique, otherwise false.</returns>
    private Task<bool> ExerciseMuscleGroupUnique(CreateExerciseMuscleGroupDto dto, CancellationToken token)
    {
        return _exerciseMuscleGroupRepository.IsUnique(dto.ExerciseTypeId, dto.MuscleGroupId, dto.MuscleFunctionId);
    }
    
}
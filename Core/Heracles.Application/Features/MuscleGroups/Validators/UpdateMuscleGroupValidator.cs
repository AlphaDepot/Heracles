using FluentValidation;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.MuscleGroups.Models;

namespace Heracles.Application.Features.MuscleGroups.Validators;

/// <summary>
/// Validates the update operation for a MuscleGroup entity.
/// </summary>
public class UpdateMuscleGroupValidator : AbstractValidator<MuscleGroup>
{
    private readonly IMuscleGroupRepository _muscleGroupRepository;

    public UpdateMuscleGroupValidator(IMuscleGroupRepository muscleGroupRepository)
    {
        _muscleGroupRepository = muscleGroupRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(MuscleGroupExists).WithMessage("Muscle group not found");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters");
        
        
        RuleFor(x => x.Name)
            .MustAsync(MuscleGroupNameUnique)
            .WithMessage("Name already exists");
    }

    /// <summary>
    /// Checks if the given muscle group name is unique.
    /// </summary>
    /// <param name="name">The name of the muscle group.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the name is unique, otherwise false.</returns>
    private Task<bool> MuscleGroupNameUnique(string name, CancellationToken token)
    {
        return _muscleGroupRepository.IsNameUnique(name);
    }

    /// <summary>
    /// Checks if a muscle group with the given ID exists.
    /// </summary>
    /// <param name="id">The ID of the muscle group.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the muscle group exists, otherwise false.</returns>
    private Task<bool> MuscleGroupExists(int id, CancellationToken token)
    {
        return _muscleGroupRepository.ItExist(id);
    }
}
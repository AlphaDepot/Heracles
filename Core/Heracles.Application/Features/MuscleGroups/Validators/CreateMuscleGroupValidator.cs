using FluentValidation;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.MuscleGroups.Models;

namespace Heracles.Application.Features.MuscleGroups.Validators;

/// <summary>
/// Validates the creation of a MuscleGroup entity.
/// </summary>
public class CreateMuscleGroupValidator : AbstractValidator<MuscleGroup>
{
    private readonly IMuscleGroupRepository _muscleGroupRepository;


    
    public CreateMuscleGroupValidator( IMuscleGroupRepository muscleGroupRepository)
    {
        _muscleGroupRepository = muscleGroupRepository;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters");
        
        RuleFor(x => x.Name)
            .MustAsync(MuscleGroupNameUnique)
            .WithMessage("Name already exists");
    }

    /// <summary>
    /// Checks if a given muscle group name is unique.
    /// </summary>
    /// <param name="name">The muscle group name to check.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task indicating whether the muscle group name is unique (true) or not (false).</returns>
    private Task<bool> MuscleGroupNameUnique(string name, CancellationToken token)
    {
        return _muscleGroupRepository.IsNameUnique(name);
    }
    
}
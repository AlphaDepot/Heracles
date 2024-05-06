using FluentValidation;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.ExercisesType.Models;

namespace Heracles.Application.Features.ExerciseTypes.Validators;

/// <summary>
/// Validates the creation of an ExerciseType entity.
/// </summary>
public class CreateExerciseTypeValidator : AbstractValidator<ExerciseType>
{
    
    private readonly IExerciseTypeRepository _exerciseTypeRepository;
    
    public CreateExerciseTypeValidator(IExerciseTypeRepository exerciseTypeRepository)
    {
        _exerciseTypeRepository = exerciseTypeRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name cannot be longer than 255 characters");

        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot be longer than 1000 characters");
        
        RuleFor(x => x.Name)
            .MustAsync(ExerciseNameUnique)
            .WithMessage("Name already exists");
        
        _exerciseTypeRepository = exerciseTypeRepository;
    }

    /// <summary>
    /// Checks if the exercise name is unique.
    /// </summary>
    /// <param name="name">The name of the exercise.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation and returns a boolean indicating if the name is unique.</returns>
    private Task<bool> ExerciseNameUnique(string name, CancellationToken token)
    {
        return _exerciseTypeRepository.IsNameUnique(name);
    }


}
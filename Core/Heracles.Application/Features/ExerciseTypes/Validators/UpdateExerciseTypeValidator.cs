using FluentValidation;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.ExercisesType.Models;

namespace Heracles.Application.Features.ExerciseTypes.Validators;

/// <summary>
/// Validator class used for updating an ExerciseType entity.
/// </summary>
public class UpdateExerciseTypeValidator : AbstractValidator<ExerciseType>
{
    private readonly IExerciseTypeRepository _exerciseTypeRepository;
    public UpdateExerciseTypeValidator(IExerciseTypeRepository exerciseTypeRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(ExerciseTypeExists).WithMessage("Exercise type not found");
        
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
    /// Checks if the given exercise name is unique.
    /// </summary>
    /// <param name="name">The name of the exercise to check.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task representing the asynchronous operation.
    /// The task result contains a boolean value indicating whether the exercise name is unique.</returns>
    private Task<bool> ExerciseNameUnique(string name, CancellationToken token)
    {
        return _exerciseTypeRepository.IsNameUnique(name);
    }

    /// <summary>
    /// Checks if the ExerciseType with the given id exists.
    /// </summary>
    /// <param name="id">The id of the ExerciseType.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Returns a task representing the asynchronous operation.
    /// The task result contains a boolean value indicating whether the ExerciseType exists.</returns>
    private Task<bool> ExerciseTypeExists(int id, CancellationToken token)
    {
        return _exerciseTypeRepository.ItExist(id);
    }
}
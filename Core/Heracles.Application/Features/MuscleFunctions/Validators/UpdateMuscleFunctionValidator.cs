using FluentValidation;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleFunctions.Models;

namespace Heracles.Application.Features.MuscleFunctions.Validators;

/// <summary>
/// 
/// </summary>
public class UpdateMuscleFunctionValidator : AbstractValidator<MuscleFunction>
{
    private readonly IMuscleFunctionRepository _muscleFunctionRepository;


    public UpdateMuscleFunctionValidator(IMuscleFunctionRepository muscleFunctionRepository)
    {
        _muscleFunctionRepository = muscleFunctionRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("The id is required.")
            .MustAsync(MuscleFunctionExists)
            .WithMessage("Muscle function not found");
            
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("The name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(100)
            .WithMessage("The name must have a maximum of 100 characters.");
        
        RuleFor(x => x.Name)
            .MustAsync(MuscleFunctionNameUnique)
            .WithMessage("Name already exists");
    }

    /// <summary>
    /// Checks if the given muscle function name is unique.
    /// </summary>
    /// <param name="name">The muscle function name</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean value indicating whether the muscle function name is unique or not.</returns>
    private Task<bool> MuscleFunctionNameUnique(string name, CancellationToken token)
    {
        return _muscleFunctionRepository.IsNameUnique(name);
    }

    /// <summary>
    /// Checks if the muscle function with the given ID exists.
    /// </summary>
    /// <param name="id">The ID of the muscle function</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean value indicating whether the muscle function exists or not.</returns>
    private Task<bool> MuscleFunctionExists(int id, CancellationToken token)
    {
        return _muscleFunctionRepository.ItExist(id);
    }
    
}
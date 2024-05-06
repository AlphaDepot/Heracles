using FluentValidation;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleFunctions.Models;

namespace Heracles.Application.Features.MuscleFunctions.Validators;

public class CreateMuscleFunctionValidator : AbstractValidator<MuscleFunction>
{
    private readonly IMuscleFunctionRepository _muscleFunctionRepository;


    public CreateMuscleFunctionValidator(IMuscleFunctionRepository muscleFunctionRepository)
    {
        _muscleFunctionRepository = muscleFunctionRepository;
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
    
    private Task<bool> MuscleFunctionNameUnique(string name, CancellationToken token)
    {
        return _muscleFunctionRepository.IsNameUnique(name);
    }
    
}
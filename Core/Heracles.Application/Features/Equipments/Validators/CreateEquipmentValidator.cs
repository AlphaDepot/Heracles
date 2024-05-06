using FluentValidation;
using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.Equipments.Models;

namespace Heracles.Application.Features.Equipments.Validators;

/// <summary>
///  Validator for creating equipment
/// </summary>
public class CreateEquipmentValidator : AbstractValidator<Equipment>
{
    private readonly IEquipmentRepository _equipmentRepository;


    public CreateEquipmentValidator(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .MinimumLength(3).WithMessage("Type must be at least 3 characters")
            .MaximumLength(255).WithMessage("Type cannot be longer than 255 characters")
            .MustAsync(EquipmentTypeUnique).WithMessage("Type already exists");
        
    }
    /// <summary>
    ///  Check if the equipment type is unique
    /// </summary>
    /// <param name="type"> Equipment type</param>
    /// <param name="token"> Cancellation token</param>
    /// <returns> True if the type is unique, false otherwise</returns>
    private Task<bool> EquipmentTypeUnique(string type, CancellationToken token)
    {
        return _equipmentRepository.IsTypeUnique(type);
    }
    
}
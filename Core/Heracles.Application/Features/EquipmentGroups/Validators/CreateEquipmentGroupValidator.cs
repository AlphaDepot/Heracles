using FluentValidation;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;

namespace Heracles.Application.Features.EquipmentGroups.Validators;

/// <summary>
///  Validator for creating equipment group.
/// </summary>
public class CreateEquipmentGroupValidator : AbstractValidator<EquipmentGroup>
{
    private readonly IEquipmentGroupRepository _equipmentGroupRepository;


    public CreateEquipmentGroupValidator(IEquipmentGroupRepository equipmentGroupRepository)
    {
        _equipmentGroupRepository = equipmentGroupRepository;
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters")
            .MustAsync(EquipmentGroupUnique).WithMessage("EquipmentGroup with name {PropertyValue} already exists");
    }
    
    /// <summary>
    ///  Check if equipment group name is unique.
    /// </summary>
    /// <param name="name">  Equipment group name. </param>
    /// <param name="token"> Cancellation token. </param>
    /// <returns> True if equipment group name is unique, otherwise false. </returns>
    private Task<bool> EquipmentGroupUnique(string name, CancellationToken token)
    {
        return _equipmentGroupRepository.IsUnique(name);
    }
    
}
using FluentValidation;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;

namespace Heracles.Application.Features.EquipmentGroups.Validators;

/// <summary>
///  Validator for updating an EquipmentGroup
/// </summary>
public class UpdateEquipmentGroupValidator : AbstractValidator<EquipmentGroup>
{
    private readonly IEquipmentGroupRepository _equipmentGroupRepository;

    public UpdateEquipmentGroupValidator(IEquipmentGroupRepository equipmentGroupRepository)
    {
        _equipmentGroupRepository = equipmentGroupRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(EquipmentGroupExists).WithMessage("EquipmentGroup not found");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters")
            .MustAsync(EquipmentGroupUnique).WithMessage("EquipmentGroup with name {PropertyValue} already exists");
    }
    
    /// <summary>
    ///  Check if EquipmentGroup exists
    /// </summary>
    /// <param name="id">  Id of the EquipmentGroup </param> 
    /// <param name="token"> Cancellation token </param>
    /// <returns> True if EquipmentGroup exists, false otherwise </returns>
    private Task<bool> EquipmentGroupExists(int id, CancellationToken token)
    {
        return _equipmentGroupRepository.ItExist(id);
    } 
    
    /// <summary>
    ///  Check if EquipmentGroup is unique
    /// </summary>
    /// <param name="name"> Name of the EquipmentGroup </param>
    /// <param name="token"> Cancellation token </param>
    /// <returns> True if EquipmentGroup is unique, false otherwise </returns>
    private Task<bool> EquipmentGroupUnique(string name, CancellationToken token)
    {
        return _equipmentGroupRepository.IsUnique(name);
    }
}

using FluentValidation;
using Heracles.Domain.EquipmentGroups.DTOs;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.Equipments.Interfaces;

namespace Heracles.Application.Features.EquipmentGroups.Validators;
 
/// <summary>
///  Validator for adding or removing equipment from equipment group.
/// </summary>
public class AddRemoveEquipmentGroupValidator : AbstractValidator<AddRemoveEquipmentGroupDto>
{
    private readonly IEquipmentGroupRepository _equipmentGroupRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public AddRemoveEquipmentGroupValidator(
        IEquipmentGroupRepository equipmentGroupRepository, IEquipmentRepository equipmentRepository)
    {
        _equipmentGroupRepository = equipmentGroupRepository;
        _equipmentRepository = equipmentRepository;
        
        RuleFor(x => x.EquipmentGroupId)
            .MustAsync(EquipmentGroupExists)
            .WithMessage("Equipment group does not exist.")
            .DependentRules(() =>
            {
                RuleFor(x => x.EquipmentId)
                    .MustAsync(EquipmentExists)
                    .WithMessage("Equipment does not exist.");
            });
    }
    
    /// <summary>
    ///   Check if equipment group exists.
    /// </summary>
    /// <param name="id"> Equipment group id. </param>
    /// <param name="token"> Cancellation token. </param>
    /// <returns> True if equipment group exists, otherwise false. </returns>
    private Task<bool> EquipmentGroupExists(int id, CancellationToken token)
    {
        return _equipmentGroupRepository.ItExist(id);
    }
    
    /// <summary>
    ///  Check if equipment exists.
    /// </summary>
    /// <param name="id"> Equipment id. </param>
    /// <param name="token"> Cancellation token. </param>
    /// <returns> True if equipment exists, otherwise false. </returns>
    private Task<bool> EquipmentExists(int id, CancellationToken token)
    {
        return _equipmentRepository.ItExist(id);
    }
}
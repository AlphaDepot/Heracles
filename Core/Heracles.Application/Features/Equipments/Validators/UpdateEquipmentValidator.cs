using FluentValidation;
using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.Equipments.Models;

namespace Heracles.Application.Features.Equipments.Validators;

/// <summary>
///  Validator for creating equipment
/// </summary>
public class UpdateEquipmentValidator : AbstractValidator<Equipment>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public UpdateEquipmentValidator(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(EquipmentExists).WithMessage("Equipment not found");
        
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

    /// <summary>
    ///  Check if the equipment exists
    /// </summary>
    /// <param name="id"> Equipment id</param>
    /// <param name="token"> Cancellation token</param>
    /// <returns> True if the equipment exists, false otherwise</returns>
    private Task<bool> EquipmentExists(int id, CancellationToken token)
    {
        return _equipmentRepository.ItExist(id);
    }
}
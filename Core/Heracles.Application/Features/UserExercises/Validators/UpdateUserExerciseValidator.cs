using FluentValidation;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.Users.Interfaces;

namespace Heracles.Application.Features.UserExercises.Validators;

/// <summary>
/// Validator for updating user exercises.
/// </summary>
public class UpdateUserExerciseValidator : AbstractValidator<UpdateUserExerciseDto>
{
    private readonly IEquipmentGroupRepository _equipmentGroupRepository;
    private readonly IUserExerciseRepository _userExerciseRepository;


    public UpdateUserExerciseValidator(
        IUserExerciseRepository userExerciseRepository, 
        IEquipmentGroupRepository equipmentGroupRepository,
        IUserService userService, string userId)
    {
        _equipmentGroupRepository = equipmentGroupRepository;
        _userExerciseRepository = userExerciseRepository;

        // Id needs to exist
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("id is required")
            .MustAsync(UserExerciseExists).WithMessage("UserExercise does not exist");
        
        // UserId needs to exist
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MustAsync((user_id, token)
                => userService.IsUserAuthorized(user_id, userId))
            .WithMessage("You are not authorized to update a user exercise for another user")
            .MustAsync((userId, token) => userService.DoesUserExist(userId))
            .WithMessage(x => $"User with id {x.UserId} does not exist");
        
        // CurrentWeight needs to be empty or a positive number
        RuleFor(x => x.CurrentWeight)
            .GreaterThanOrEqualTo(0).WithMessage("CurrentWeight must be a positive number");
        
        // PersonalRecord needs to be empty or a positive number
        RuleFor(x => x.PersonalRecord)
            .GreaterThanOrEqualTo(0).WithMessage("PersonalRecord must be a positive number");
        
        // DurationInSeconds needs to be a positive number
        RuleFor(x => x.DurationInSeconds)
            .GreaterThanOrEqualTo(0).WithMessage("DurationInSeconds must be a positive number");
        
        // Repetitions needs to be a positive number
        RuleFor(x => x.Repetitions)
            .GreaterThanOrEqualTo(0).WithMessage("Repetitions must be a positive number");
        
        // Sets needs to be a positive number
        RuleFor(x => x.Sets)
            .GreaterThanOrEqualTo(0).WithMessage("Sets must be a positive number");
        
        
        // EquipmentGroupId needs to exist or be 0
        // Check if the EquipmentGroup was provided with an if
        When(x => x.EquipmentGroupId != 0, () =>
        {
            RuleFor(x => x.EquipmentGroupId)
                .MustAsync(EquipmentGroupExist).WithMessage("EquipmentGroup does not exist");
        });
        
    }
    
    
    
    /// <summary>
    ///  Check if the UserExercise exists
    /// </summary>
    /// <param name="id"> The UserExercise id to be checked </param>
    /// <param name="token"> The cancellation token </param>
    /// <returns>  True if the UserExercise exists, false otherwise </returns>
    private Task<bool> UserExerciseExists(int id, CancellationToken token)
    {
        return _userExerciseRepository.ItExist(id);
    }
    
    /// <summary>
    ///  Check if the EquipmentGroup exists
    /// </summary>
    /// <param name="id"> The EquipmentGroup id to be checked </param>
    /// <param name="token"> The cancellation token </param>
    /// <returns></returns>
    private Task<bool> EquipmentGroupExist(int id, CancellationToken token)
    {
        return _equipmentGroupRepository.ItExist(id);
    }
}
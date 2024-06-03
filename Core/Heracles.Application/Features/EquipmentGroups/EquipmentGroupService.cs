using System.Linq.Expressions;
using Heracles.Application.Features.EquipmentGroups.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.EquipmentGroups.DTOs;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.Domain.Equipments.Interfaces;

namespace Heracles.Application.Features.EquipmentGroups;

/// <summary>
/// Represents a service for managing equipment groups.
/// </summary>
public class EquipmentGroupService : IEquipmentGroupService
{
    /// <summary>
    /// Provides logging functionality for the EquipmentGroupService class.
    /// </summary>
    /// <typeparam name="EquipmentGroupService">The type of the class that is being logged.</typeparam>
    private readonly IAppLogger<EquipmentGroupService> _logger;

    /// <summary>
    /// Represents a service that provides operations for managing equipment groups.
    /// </summary>
    private readonly IEquipmentGroupRepository _repository;

    /// <summary>
    /// Represents an interface for interacting with the equipment repository.
    /// </summary>
    private readonly IEquipmentRepository _equipmentRepository;

    /// <summary>
    /// Represents a service for managing equipment groups.
    /// </summary>
    public EquipmentGroupService(
        IAppLogger<EquipmentGroupService> logger,
        IEquipmentGroupRepository repository,
        IEquipmentRepository equipmentRepository)
    {
        _logger = logger;
        _repository = repository;
        _equipmentRepository = equipmentRepository;
    }


    /// <summary>
    /// Retrieves equipment groups based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <returns>A domainResponse containing the equipment groups that match the query.</returns>
    public async Task<DomainResponse<QueryResponseDto<EquipmentGroup>>> GetAsync(QueryRequestDto query)
    {
        
        var filter = EquipmentGroup.GetFilterExpression(query?.SearchTerm);
        var sortExpressions = EquipmentGroup.GetSortExpression();
        
        var  queryHelper = new QueryHelper().CreateQueriable(query, sortExpressions, filter);
        var result = await _repository.GetAsync(queryHelper);
      

        return DomainResponse.Success(result);
    }

    /// <summary>
    /// Retrieves an instance of EquipmentGroup from the repository by its ID.
    /// </summary>
    /// <param name="id">The ID of the EquipmentGroup to retrieve.</param>
    /// <returns>A <see cref="ResponseDtoDto{TValue}"/> containing the retrieved EquipmentGroup instance or an error message.</returns>
    public async Task<DomainResponse<EquipmentGroup>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<EquipmentGroup>());
            return DomainResponse.Failure<EquipmentGroup>(EntityErrorMessage<EquipmentGroup>.BadRequest());
        }
        
        var result = await _repository.GetEntityByIdAsync(id);
        
        if (result == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<EquipmentGroup>(id));
            return DomainResponse.Failure<EquipmentGroup>(EntityErrorMessage<EquipmentGroup>.NotFound(id));
        }
        
        _logger.LogInformation(ServiceMessages.EntityRetrieved<EquipmentGroup>(id));
        return DomainResponse.Success(result);
    }

    /// <summary>
    /// Creates a new equipment group asynchronously.
    /// </summary>
    /// <param name="entity">The equipment group entity to create.</param>
    /// <returns>A <see cref="Task{DomainResponse{int}}"/> indicating the asynchronous operation, with a domainResponse containing the ID of the created equipment group if successful.</returns>
    public async Task<DomainResponse<int>> CreateAsync(EquipmentGroup entity)
    {
        var validator = new CreateEquipmentGroupValidator(_repository);
        var validationResult = await validator.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<EquipmentGroup>(validationResult.ToDictionary()));
            return DomainResponse.Failure<int>(EntityErrorMessage<EquipmentGroup>.BadRequest(validationResult.ToDictionary()));
        }
        
        var id = await _repository.CreateEntityAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityCreated<EquipmentGroup>(id));
        return DomainResponse.Success(id);
        
    }

    /// <summary>
    /// Updates an equipment group.
    /// </summary>
    /// <param name="entity">The equipment group object to update.</param>
    /// <returns>A domainResponse indicating if the update operation was successful.</returns>
    public async Task<DomainResponse<bool>> UpdateAsync(EquipmentGroup entity)
    {
        var validator = new UpdateEquipmentGroupValidator(_repository);
        var validationResult = await validator.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<EquipmentGroup>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<EquipmentGroup>.BadRequest(validationResult.ToDictionary()));
        }
        
        await _repository.UpdateEntityAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityUpdated<EquipmentGroup>(entity.Id));
        
        return DomainResponse.Success(true);
        
    }

    /// <summary>
    /// Deletes an equipment group by its ID.
    /// </summary>
    /// <param name="id">The ID of the equipment group to delete.</param>
    /// <returns>A <see cref="ResponseDtoDto{TValue}"/> indicating whether the delete operation was successful or not.</returns>
    public async Task<DomainResponse<bool>> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<EquipmentGroup>());
            return DomainResponse.Failure<bool>(EntityErrorMessage<EquipmentGroup>.BadRequest());
        }
        
        var exists = await _repository.ItExist(id);
        if (!exists)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<EquipmentGroup>(id));
            return DomainResponse.Failure<bool>(EntityErrorMessage<EquipmentGroup>.NotFound(id));
        }
        
        await _repository.DeleteEntityAsync(id);
        _logger.LogInformation(ServiceMessages.EntityDeleted<EquipmentGroup>(id));
        return DomainResponse.Success(true);
    }

    /// <summary>
    /// Adds equipment to an equipment group asynchronously.
    /// </summary>
    /// <param name="entityDto">The DTO containing the equipment group ID and equipment ID.</param>
    /// <returns>A domainResponse indicating the result of the operation.</returns>
    public async Task<DomainResponse<bool>> AddEquipmentAsync(AddRemoveEquipmentGroupDto entityDto)
    {
        var validator = new AddRemoveEquipmentGroupValidator(_repository, _equipmentRepository);
        var validationResult = validator.ValidateAsync(entityDto);
        
        if (!validationResult.Result.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<EquipmentGroup>(validationResult.Result.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<EquipmentGroup>.BadRequest(validationResult.Result.ToDictionary()));
        }
        
        var equipmentGroup = await _repository.GetEntityByIdAsync(entityDto.EquipmentGroupId);
        var equipment = await _equipmentRepository.GetEntityByIdAsync(entityDto.EquipmentId);
        
        equipmentGroup.Equipments?.Add(equipment);
        await _repository.UpdateEntityAsync(equipmentGroup);
        
        _logger.LogInformation(ServiceMessages.EntityUpdated<EquipmentGroup>(equipmentGroup.Id));
        return DomainResponse.Success(true);
    }

    /// <summary>
    /// Removes an equipment from an equipment group asynchronously.
    /// </summary>
    /// <param name="entityDto">The object containing the equipment group ID and equipment ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains a <see cref="ResponseDtoDto{TValue}"/> with a boolean indicating the success of the operation.</returns>
    public async Task<DomainResponse<bool>> RemoveEquipmentAsync(AddRemoveEquipmentGroupDto entityDto)
    {
        var validator = new AddRemoveEquipmentGroupValidator(_repository, _equipmentRepository);
        var validationResult = validator.ValidateAsync(entityDto);
        
        if (!validationResult.Result.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<EquipmentGroup>(validationResult.Result.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<EquipmentGroup>.BadRequest(validationResult.Result.ToDictionary()));
        }
        
        var equipmentGroup = await _repository.GetEntityByIdAsync(entityDto.EquipmentGroupId);
        var equipment = await _equipmentRepository.GetEntityByIdAsync(entityDto.EquipmentId);
        
        equipmentGroup.Equipments?.Remove(equipment);
        await _repository.UpdateEntityAsync(equipmentGroup);
        
        _logger.LogInformation(ServiceMessages.EntityUpdated<EquipmentGroup>(equipmentGroup.Id));
        return DomainResponse.Success(true);
    }
}
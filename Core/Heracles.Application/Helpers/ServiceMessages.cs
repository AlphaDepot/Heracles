using Heracles.Domain.Abstractions;
using Heracles.Domain.Abstractions.Entities;

namespace Heracles.Application.Helpers;

public static class ServiceMessages
{
    public static string EntityRetrieved<T>(int id) where T : BaseEntity
    {
        return $"{typeof(T)} with {id} retrieved!";
    }
    
    public static string EntityCreated<T>(int id) where T : BaseEntity
    {
        return $"{typeof(T)} with {id} created!";
    }

    public static string EntityUpdated<T>(int id) where T : BaseEntity
    {
        return $"{typeof(T)} with {id} updated!";
    }


    public static string EntityDeleted<T>(int id) where T : BaseEntity
    {
        return $"{typeof(T)} with {id} deleted!";
    }
    

    public static string EntityNotFound<T>(int entityId) where T : BaseEntity
    {
        return $"{typeof(T)} with id of {entityId} not found!";
    }
    
    public static string EntityNotFound<T>(string entityId) where T : BaseEntity
    {
        return $"{typeof(T)} with id of {entityId} not found!";
    }

    public static string EntityConcurrencyMismatch<T>() where T : BaseEntity
    {
        return $"{typeof(T)} has been modified since it was last retrieved!";
    }

    public static string EntityIdInvalid<T>() where T : BaseEntity
    {
        return $"{typeof(T)} ID is invalid!";
    }
    
    
    public static string EntityValidationFailure<T>(IDictionary<string,string[]> errors) where T : BaseEntity
    {
        return $"{typeof(T)} validation failed: {
            string.Join(", ", 
                errors.Select(x => $"{x.Key}: {string.Join(", ", x.Value)}"))
        }";
    }
    
    
}
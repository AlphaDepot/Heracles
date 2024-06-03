using Heracles.Domain.Abstractions.Entities;

namespace Heracles.Domain.Abstractions.Errors;

public static class EntityErrorMessage<T> where T : BaseEntity
{
    public static Error BadRequest() =>
        new Error($"Invalid{typeof(T).Name}", $"The {typeof(T).Name} is invalid.");

    public static Error BadRequest(IDictionary<string, string[]> errors) =>
        new Error($"Invalid{typeof(T).Name}", $"The {typeof(T).Name} is invalid.", errors);

    public static Error NotFound() =>
        new Error($"{typeof(T).Name}NotFound", $"The {typeof(T).Name} was not found.");

    public static Error NotFound(int id) =>
        new Error($"{typeof(T).Name}NotFound", $"The {typeof(T).Name} with id {id} was not found.");
    
    public static Error NotFound(string id) =>
        new Error($"{typeof(T).Name}NotFound", $"The {typeof(T).Name} with id {id} was not found.");
    
    // Unauthorized
    public static Error Unauthorized() =>
        new Error($"{typeof(T).Name}Unauthorized", $"You are not authorized to access the {typeof(T).Name}.");
}
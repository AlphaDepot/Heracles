using System.Security.Claims;
using Heracles.Domain.Users.Interfaces;
using Heracles.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;


namespace Heracles.Infrastructure.Middlewares;

/// <summary>
///  Middleware to fetch user information from claims and store it in a convenient location (e.g., ThreadLocal)
/// as well as create a new user if not found in the database
/// </summary>
public class UserInformationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private string? ObjectId { get; set; }
    private List<Claim> Claims { get; set; } = new();

    
    
    public UserInformationMiddleware(RequestDelegate next, IMemoryCache cache,  IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _cache = cache;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
   /// <summary>
   ///  Middleware to fetch user information from claims and store it in a convenient location (e.g., ThreadLocal)
   /// </summary>
   /// <param name="context"> The current HttpContext.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        Claims = context.User.Claims.ToList();
        ObjectId = Claims.FirstOrDefault(c => c.Type == ClaimConstants.ObjectId)?.Value;

        // If the user id is not found in the claims, continue to the next middleware
        if (string.IsNullOrEmpty(ObjectId))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("No user id found in claims.");
            return;
        }
        
        // Check if user information is cached
        if (_cache.TryGetValue(ObjectId, out var cachedUser))
        {
            // Store the user in the context items
            context.Items["User"] = cachedUser;
            await _next(context);
            return;
        }

        // Create a new scope to get the user service
        // This is necessary because the middleware is singleton and the service is scoped 
        using var scope = _serviceScopeFactory.CreateScope();
        // Get the user service from the scope
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        
        // Get user from database by user id
        var userResponse = await userService.GetUserByUserIdAsync(ObjectId);
        // If user is found, store it in the context items
        if (userResponse.IsSuccess)
        {
            // Cache the retrieved  user information
            _cache.Set(ObjectId, userResponse.Value, TimeSpan.FromHours(24));
            // Store the user in the context items
            context.Items["User"] = userResponse.Value; 
            await _next(context);
            return;
        }
        
        //  If user is not found, create a new user from the claims
        var newUser = ClaimsExtractor.ExtractUser(Claims);
        if (newUser == null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Failed to extract user from claims.");
            return;
        }
        
        // Create the new user in the database
        var createUser =  await userService.CreateUserAsync(newUser);
        // If user creation fails, return an error
        if (createUser.IsFailure)
        {
            context.Response.StatusCode = createUser.StatusCode;
            await context.Response.WriteAsync("Failed to create user from claims.");
            return;
        }
        
        // Cache newly created user information
        _cache.Set(ObjectId, createUser.Value, TimeSpan.FromHours(24));
        // If user creation is successful, store the user in the context items
        context.Items["User"] = createUser.Value;
        await _next(context);
    }
    
}
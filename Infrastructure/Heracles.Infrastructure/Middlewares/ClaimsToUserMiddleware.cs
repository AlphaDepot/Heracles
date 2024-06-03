using System.Security.Claims;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;
using Heracles.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
namespace Heracles.Infrastructure.Middlewares;

public class ClaimsToUserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public ClaimsToUserMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var userId = ExtractUserId(context);
        if (string.IsNullOrEmpty(userId))
        {
            await WriteResponse(context, 400, "No user id found in claims.");
            return;
        }

        var user = await GetUser(userId, context.User.Claims );
        if (user == null)
        {
            await WriteResponse(context, 400, "Failed to create user.");
            return;
        }

        AddUserToContext(context, user);
        await _next(context);
    }

    private static string ExtractUserId(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimConstants.ObjectId);
        return userId ?? string.Empty;
    }

    private async Task<User?> GetUser(string userId, IEnumerable<Claim> claims)
    {
        return await GetUserFromStorage(userId) ?? await CreateUserFromClaims(claims);
    }

    private async Task<User?> GetUserFromStorage(string userId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var userResponse = await userService.GetUserByUserIdAsync(userId);
        return userResponse.IsSuccess ? userResponse.Value : null;
    }

    private async Task<User?> CreateUserFromClaims(IEnumerable<Claim> claims)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var userFromClaims = GetUserFromClaims(claims);
        if (userFromClaims == null) return null;

        var response = await userService.CreateUserAsync(userFromClaims);
        if (response.IsFailure) return null;
        userFromClaims.Id = response.Value;

        return userFromClaims;
    }

    private static void AddUserToContext(HttpContext context, User user)
    {
        context.Items["User"] = user;
    }
    
    private static User? GetUserFromClaims(IEnumerable<Claim> claims)
    {
        var userFromClaims = new UserFromClaims(claims);
        return userFromClaims.TryGet();
    }
    
    private static async Task WriteResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
    
}
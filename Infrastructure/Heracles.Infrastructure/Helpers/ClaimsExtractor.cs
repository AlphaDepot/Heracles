using System.Security.Claims;
using Heracles.Domain.Users.Models;
using static Microsoft.Identity.Web.ClaimConstants;

namespace Heracles.Infrastructure.Helpers;

/// <summary>
///  Extracts information from claims
/// </summary>
public abstract class ClaimsExtractor
{
    
    /// <summary>
    ///  Extracts user information from claims
    /// </summary>
    /// <param name="claims"> The claims to extract user information from.</param>
    /// <returns> A user object with the extracted information.</returns>
    public static User? ExtractUser(IEnumerable<Claim> claims)
    {
        var claimDictionary = claims.ToDictionary(c => c.Type, c => c.Value);

        // Extract roles from claims if present otherwise extract single role if present otherwise return empty list 
        var userRoles = claimDictionary.TryGetValue(Roles, out var rolesValue)
            ? rolesValue.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            : claimDictionary.TryGetValue(Role, out var roleValue) 
                ? new List<string> { roleValue } 
                : new List<string>();

        var userId = claimDictionary.TryGetValue(ObjectId, out var objectId) ? objectId : null;

        if (string.IsNullOrEmpty(userId))
            return null;

        return new User 
        {
            Id = 0,
            UserId = userId,
            Name = claimDictionary.TryGetValue(Name, out var nameValue) ? nameValue : throw new ArgumentException("Name claim is missing."),
            Email = claimDictionary.TryGetValue(PreferredUserName, out var emailValue) ? emailValue : throw new ArgumentException("Email claim is missing."),
            Roles = userRoles
        };
    }
}
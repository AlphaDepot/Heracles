using System.Security.Claims;
using Heracles.Domain.Enums;
using Heracles.Domain.Models;
using static Microsoft.Identity.Web.ClaimConstants;
namespace Heracles.Infrastructure.Claims;

public sealed class UserFromClaims
{
    private readonly Dictionary<string, string> _claimsDictionary;
    public UserFromClaims(IEnumerable<Claim> claims)
    {
        _claimsDictionary = claims.ToDictionary(c => c.Type, c => c.Value);
    }
    
    public User? TryGet()
    {
        var userId = GetClaimValueFrom(ObjectId);
        var userName = GetClaimValueFrom(Name);
        var userEmail = GetClaimValueFrom(PreferredUserName);
        var userRoles = GetUserRolesOrDefault();
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
            return null;

        return new User(userId, userName, userEmail, userRoles);
    }

    private List<UserRoles>? GetUserRolesOrDefault()
    {
        var rolesStringList = RolesStringListFromClaims();
        if (rolesStringList == null)
            return null;

        return rolesStringList
            .Where(role => Enum.TryParse<UserRoles>(role, true, out var _))
            .Select(role => Enum.Parse<UserRoles>(role, true))
            .ToList();
    }


    private List<string>? RolesStringListFromClaims()
    {
        var rolesValue = GetRolesValue();
        if (rolesValue != null)
        {
            return rolesValue.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        var roleValue = GetRoleValue();
        return roleValue != null ? new List<string> { roleValue } : null;
    }

    private string? GetClaimValueFrom(string claimType)
    {
        return _claimsDictionary.GetValueOrDefault(claimType);
    }
    
    private string? GetRolesValue()
    {
        _claimsDictionary.TryGetValue(Roles, out var rolesValue);
        return rolesValue;
    }

    private string? GetRoleValue()
    {
        _claimsDictionary.TryGetValue(Role, out var roleValue);
        return roleValue;
    }
    
}
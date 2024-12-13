using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace Api.IntegrationTest.Helpers;

/// <summary>
/// Helper class for creating and validating JSON Web Tokens (JWT).
/// </summary>
public abstract class JwtTokenHelper
{
    public const string SecretKey = "this_is_a_very_long_secret_key_that_is_at_least_16_characters";
    public const string Issuer = "your_issuer_here";
    public const string Audience = "your_audience_here";

    /// <summary>
    /// Creates a fake JWT (JSON Web Token) token using the provided admin claims.
    /// It is important to note that the claims variables used in this method
    /// should match the ones used in the ClaimsExtractor class. If they do not match,
    /// the ExtractUser method will not be able to extract the user information.
    /// This can result in an "unable to extract user information" error being thrown.
    /// </summary>
    /// <returns>A string representing the fake JWT token.</returns>
    public static string CreateFakeJwtToken()
    {
        var claims = GetAdminClaim();
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Retrieves the admin claims.
    ///  It is important that the claims Variables are the same as the ones in the ClaimsExtractor class.
    ///  If they are not the same, the ExtractUser method will not be able to extract the user information.
    ///  And a unable to extract user information error will be thrown.
    /// </summary>
    /// <returns>A list of admin claims.</returns>
    public static  List<Claim> GetAdminClaim()
    {
        var claims = new List<Claim>
        {
            new(ClaimConstants.Name, "Test User"),
           new(ClaimConstants.ObjectId, "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f21"),
            new(ClaimConstants.PreferredUserName, "test@test.com"),
            new(ClaimTypes.Role, "Admin")
        };

        return claims;
    }

}

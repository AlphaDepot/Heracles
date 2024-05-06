using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using Heracles.API.IntegrationTest.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Heracles.API.IntegrationTest.Handlers;

/// <summary>
///  This class is used to handle the authentication for testing.
/// </summary>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }

    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

    /// <summary>
    ///  This method is used to handle the authentication process.
    /// </summary>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if the Authorization header is present and has a valid Bearer token value
        if (!Request.Headers.TryGetValue("Authorization", out var authorization)
            || !AuthenticationHeaderValue.TryParse(authorization, out var headerValue)
            || !"Bearer".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }
           
     
        // Get the token from the Authorization header
        var jwtHandler = new JwtSecurityTokenHandler();
        var token = headerValue.Parameter;
        var secretKey = JwtTokenHelper.SecretKey; // Replace with your actual secret key

        
        // Validate the token
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };

        try
        {
            // Validate the token and create the authentication ticket
            var claimsPrincipal = jwtHandler.ValidateToken(token, parameters, out var _);
            // Create the authentication ticket
            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception)
        {
            // Return failure result if the token is invalid
            return AuthenticateResult.Fail("Invalid JWT token");
        }
        
    }
    
}
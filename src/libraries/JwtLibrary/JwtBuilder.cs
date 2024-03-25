using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JwtLibrary;

public class JwtBuilder : IJwtBuilder
{
    private readonly JwtOptions _jwtOptions;

    public JwtBuilder(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GetToken(string userId)
    {
        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("userId", userId)
        };

        var expirationDate = DateTime.Now.AddMinutes(_jwtOptions.ExpiryMinutes);

        var token = new JwtSecurityToken(
            claims: claims,
            issuer: "http://localhost:5261",
            signingCredentials: signinCredentials,
            expires: expirationDate);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string ValidateToken(string token)
    {
        var principal = GetPrincipal(token);
        if (principal == null)
        {
            return string.Empty;
        }

        ClaimsIdentity? identity;
        try
        {
            identity = (ClaimsIdentity?)principal.Identity;
        }
        catch (NullReferenceException)
        {
            return string.Empty;
        }
        var userIdClaim = identity?.FindFirst("userId");
        if (userIdClaim == null)
        {
            return string.Empty;
        }
        var userId = userIdClaim.Value;
        return userId;
    }

    private ClaimsPrincipal? GetPrincipal(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            if (jwtToken == null)
            {
                return null;
            }
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var parameters = new TokenValidationParameters()
            {
                SaveSigninToken = true,
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5261",
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            IdentityModelEventSource.ShowPII = true;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}


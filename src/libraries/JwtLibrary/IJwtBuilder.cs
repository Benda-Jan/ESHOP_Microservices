using System;
using System.Security.Claims;
namespace JwtLibrary;

public interface IJwtBuilder
{
    string GetToken(string userId);
    string ValidateToken(string token);
    ClaimsPrincipal? GetPrincipal(string token);
}


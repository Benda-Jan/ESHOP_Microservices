using JwtLibrary;
using Microsoft.AspNetCore.Http;
//using Microsoft.IdentityModel.Tokens;

namespace AuthorizationMiddleware;

public class JwtMiddleware : IMiddleware
{
    private readonly IJwtBuilder _jwtBuilder;

    public JwtMiddleware(IJwtBuilder jwtBuilder)
    {
        _jwtBuilder = jwtBuilder;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var bearer = context.Request.Headers["Authorization"].ToString();
        var token = bearer.Replace("Bearer ", string.Empty);

        if (!string.IsNullOrEmpty(token))
        {
            var userId = _jwtBuilder.ValidateToken(token);

            if (!string.IsNullOrEmpty(userId))
            {
                context.Items["userId"] = userId;
            }
            else
            {
                context.Response.StatusCode = 401;
            }

        }

        await next(context);
    }
}


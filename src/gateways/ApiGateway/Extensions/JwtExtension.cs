using System;
using JwtLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthorizationMiddleware;

namespace ApiGateway.Extensions;

public static class JwtExtension
{
    public static void AddJwtExtension
    (this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("jwt");
        var options = section.Get<JwtOptions>();
        var key = Encoding.UTF8.GetBytes(options?.SecretKey ?? "");
        section.Bind(options);
        services.Configure<JwtOptions>(section);

        services.AddSingleton<IJwtBuilder, JwtBuilder>();
        services.AddTransient<JwtMiddleware>();

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        services.AddAuthorization(x =>
        {
            x.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });
    }
}


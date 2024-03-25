using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using JwtLibrary;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Catalog.API.Extensions;

	public static class JwtExtension
	{
    public static void AddJwtExtension (this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("JWT");
        var options = section.Get<JwtOptions>();
        var key = Encoding.UTF8.GetBytes(options?.SecretKey ?? "");
        section.Bind(options);
        services.Configure<JwtOptions>(section);

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
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5261",
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


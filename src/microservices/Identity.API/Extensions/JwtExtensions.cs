using System;
using System.Text;
using JwtLibrary;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Extensions;

public static class JwtExtensions
{
	public static void AddJwtExtension(this IServiceCollection services, IConfiguration configuration)
	{
        var options = new JwtOptions();
        var section = configuration.GetSection("JWT");
        section.Bind(options);

        services.Configure<JwtOptions>(section);

        services.AddScoped<IJwtBuilder, JwtBuilder>();

        services.AddAuthentication()
        .AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5261",
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey
                                   (Encoding.UTF8.GetBytes(options.SecretKey))
            };
        });
    }
}


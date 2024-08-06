using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Main;
public static class WebApplicationBuilderExtension {
    public static void SetAuthentication(this WebApplicationBuilder builder) {
        var validIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER") ?? "";
        var validAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE") ?? "";
        var issuerSigningKey = Environment.GetEnvironmentVariable("JWT_ISSUER_SIGNING_KEY") ?? "";
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey))
                };
            });
    }

    public static void SetCors(this WebApplicationBuilder builder) {
        var feUrl = Environment.GetEnvironmentVariable("FE_URL") ?? "";
        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(
                policy => {
                    policy.WithOrigins(feUrl)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin();
                }
            );
        });
    }
}
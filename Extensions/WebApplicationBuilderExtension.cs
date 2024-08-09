using System;
using System.Text;
using Main.Consts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Main;
public static class WebApplicationBuilderExtension {
    public static void SetAuthentication(this WebApplicationBuilder builder) {
        var validIssuer = Environment.GetEnvironmentVariable(EnvironmentVariables.Issuer) ?? "";
        var validAudience = Environment.GetEnvironmentVariable(EnvironmentVariables.Audience) ?? "";
        var issuerSigningKey = Environment.GetEnvironmentVariable(EnvironmentVariables.SigningKey) ?? "";
        
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
        var feUrl = Environment.GetEnvironmentVariable(EnvironmentVariables.FrontendUrl) ?? "";
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

    public static void SupressInvalidFilter(this WebApplicationBuilder builder) {
        builder.Services.Configure<ApiBehaviorOptions>(options => {
            options.SuppressModelStateInvalidFilter = true;
        });
    }
}
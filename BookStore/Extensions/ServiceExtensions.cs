﻿using Application.Contracts;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastucture.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(builder =>
        {
            builder.AddPolicy("AllowAll", p =>
                p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

        });
    }
    //public static void ConfigureOpenApi(this IServiceCollection services) =>
    //services.AddSwaggerGen(setup =>
    //{
    //    setup.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    //    setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //    {
    //        In = ParameterLocation.Header,
    //        Description = "Enter JWT Bearer token **_only_**",
    //        Name = "Authorization",
    //        Type = SecuritySchemeType.Http,
    //        Scheme = "bearer",
    //        BearerFormat = "JWT"
    //    });

    //    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    //    {
    //        {
    //            new OpenApiSecurityScheme
    //            {
    //                Reference = new OpenApiReference
    //                {
    //                    Type = ReferenceType.SecurityScheme,
    //                    Id = "Bearer"
    //                },
    //                Scheme = "bearer",
    //                Name = "Authorization",
    //                In = ParameterLocation.Header,
    //            },
    //            new List<string>()
    //        }
    //    });
    //});

    public static void ConfigureOpenApi(this IServiceCollection services) =>
        services.AddEndpointsApiExplorer()
                .AddSwaggerGen(setup =>
                {
                    setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Place to add JWT with Bearer",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    });

                    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                    });
                });


    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped(provider => new Lazy<IAuthenticationService>(() => provider.GetRequiredService<IAuthenticationService>()));
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
       
        var secretkey = configuration["secretkey"];
        ArgumentNullException.ThrowIfNull(secretkey, nameof(secretkey));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            ArgumentNullException.ThrowIfNull(jwtSettings, nameof(jwtSettings));

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey))
            };

        });
    }
}

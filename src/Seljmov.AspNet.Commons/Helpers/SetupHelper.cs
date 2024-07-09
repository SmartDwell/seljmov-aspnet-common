using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Seljmov.AspNet.Commons.Options;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace Seljmov.AspNet.Commons.Helpers;

/// <summary>
/// Helper for setup application.
/// </summary>
public static class SetupHelper
{
    /// <summary>
    /// Build web application.
    /// </summary>
    /// <param name="builder">Web application builder.</param>
    /// <param name="buildOptions">Build options.</param>
    /// <returns>Web application.</returns>
    public static WebApplication BuildWebApplication(this WebApplicationBuilder builder, BuildOptions? buildOptions = null)
    {
        buildOptions ??= new BuildOptions();

        ConfigureJwtAuthentication(builder, buildOptions);

        ConfigureSwagger(builder, buildOptions);

        builder.Services.AddControllers();
        builder.Services.AddSingleton<JwtReader>();

        var app = builder.Build();
        var logger = app.Logger;

        LogApplicationStartup(logger, app);

        ConfigureMiddlewares(app, buildOptions);

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        return app;
    }

    private static void ConfigureJwtAuthentication(WebApplicationBuilder builder, BuildOptions buildOptions)
    {
        if (!buildOptions.UseJwtAuthentication) return;

        var jwtOptionsSection = builder.Configuration.GetSection(nameof(JwtOptions));
        var jwtOptions = jwtOptionsSection.Get<JwtOptions>() ?? throw new Exception("JwtOptions is null");

        builder.Services.AddOptions<JwtOptions>().Bind(jwtOptionsSection);
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey(),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                };
            });

        if (buildOptions.AuthenticationPolicies.Any())
        {
            builder.Services.AddAuthorization(options =>
            {
                foreach (var policy in buildOptions.AuthenticationPolicies)
                {
                    options.AddPolicy(policy, policyBuilder => policyBuilder.RequireClaim("Permission", policy));
                }
            });
        }
    }

    private static void ConfigureSwagger(WebApplicationBuilder builder, BuildOptions buildOptions)
    {
        var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
        var appVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = appName, Version = appVersion });

            if (buildOptions.UseJwtAuthentication)
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        Array.Empty<string>()
                    }
                });
            }

            c.AddEnumsWithValuesFixFilters();
            c.UseAllOfToExtendReferenceSchemas();
            Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly)
                     .ToList()
                     .ForEach(xmlFile => c.IncludeXmlComments(xmlFile));
        });
    }

    private static void LogApplicationStartup(ILogger logger, WebApplication app)
    {
        var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
        var appVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString();

        logger.LogInformation("Starting {AppName} v{AppVersion}", appName, appVersion);

        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Use development exception page.");
        }

        logger.LogInformation("Use Swagger.");
    }

    private static void ConfigureMiddlewares(WebApplication app, BuildOptions buildOptions)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerUI(c =>
            {
                var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", appName);
                c.RoutePrefix = "swagger";
            });
        }

        app.UseSwagger();

        app.UseRouting();

        if (buildOptions.UseCors)
        {
            app.UseCors(x => x.AllowAnyMethod()
                              .AllowAnyHeader()
                              .SetIsOriginAllowed(origin => true)
                              .AllowCredentials());
        }

        if (buildOptions.UseJwtAuthentication)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
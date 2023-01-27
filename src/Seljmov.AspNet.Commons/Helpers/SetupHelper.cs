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
/// Помощник для конфигурации сервиса
/// </summary>
public static class SetupHelper
{
    /// <summary>
    /// Конфигурация сервиса
    /// </summary>
    /// <param name="builder">Билдер</param>
    /// <returns>Сконфигурированное приложение</returns>
    public static WebApplication BuildWebApplication(this WebApplicationBuilder builder)
    {
        var jwtOptionsSection = builder.Configuration.GetSection(nameof(JwtOptions));
        var jwtOptions = jwtOptionsSection.Get<JwtOptions>();

        if (jwtOptions == null)
            throw new Exception("JwtOptions is null");
        
        builder.Services.AddOptions<JwtOptions>()
            .Bind(jwtOptionsSection);
            
        builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtOptions?.GetSymmetricSecurityKey(),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions?.Issuer,
                    ValidateAudience = true,
                    ValidAudience = $"*.{jwtOptions?.Issuer}",
                    ValidateLifetime = true
                };
            });
        
        var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
        var appVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = appName,
                Version = appVersion
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()  
            {  
                Name = "Authorization",  
                Type = SecuritySchemeType.ApiKey,  
                Scheme = "Bearer",  
                BearerFormat = "JWT",  
                In = ParameterLocation.Header,  
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"", 
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement  
            {  
                {  
                    new OpenApiSecurityScheme  
                    {  
                        Reference = new OpenApiReference  
                        {  
                            Type = ReferenceType.SecurityScheme,  
                            Id = "Bearer"  
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }  
            });

            c.AddEnumsWithValuesFixFilters();
            c.UseAllOfToExtendReferenceSchemas();
            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
            xmlFiles.ForEach(xmlFile => c.IncludeXmlComments(xmlFile));
        });
        
        builder.Services.AddControllers();
        builder.Services.AddSingleton<JwtReader>();

        var app = builder.Build();
        var logger = app.Logger;

        logger.LogInformation("Starting {AppName}...", appName);

        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation($"Use development exception page");
            app.UseDeveloperExceptionPage();
        }

        logger.LogInformation($"Use Swagger.");
        app.UseSwagger();

        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation($"Use Swagger UI.");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", appName);
                c.RoutePrefix = "swagger";
            });
        }

        app.UseRouting();

        logger.LogInformation($"Use JWT authorization.");
        app.UseAuthentication();
        app.UseAuthorization();

        logger.LogInformation($"Use controllers.");
        app.UseEndpoints(endpoints => endpoints.MapControllers());

        return app;
    }
}
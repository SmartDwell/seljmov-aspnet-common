using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Seljmov.AspNet.Commons.Options;

namespace Seljmov.AspNet.Commons.Helpers;

/// <summary>
/// Класс для чтения Jwt
/// </summary>
public class JwtReader
{
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly ILogger<JwtReader> _logger;

    /// <summary>
    /// Конструктор класса <see cref="JwtReader"/>
    /// </summary>
    /// <param name="jwtOptions">Настройки jwt</param>
    /// <param name="logger">Логгер</param>
    public JwtReader(IOptions<JwtOptions> jwtOptions, ILogger<JwtReader> logger)
    {
        _jwtOptions = jwtOptions;
        _logger = logger;
    }

    /// <summary>
    /// Прочитать Jwt
    /// </summary>
    /// <param name="token">Jwt</param>
    /// <param name="claims">Данные пользователя</param>
    /// <param name="validTo">Дата валидности</param>
    /// <returns></returns>
    public bool ReadAccessToken(string token, out ClaimsPrincipal? claims, out DateTime validTo)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validations = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _jwtOptions.Value.GetSymmetricSecurityKey(),
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtOptions.Value.Audience,
            ValidateLifetime = true
        };
        
        try
        {
            claims = tokenHandler.ValidateToken(token, validations, out var validatedToken);
            validTo = validatedToken.ValidTo;
            return true;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(message: ex.ToString());
        }
        
        claims = null;
        validTo = DateTime.MinValue;
        return false;
    }
}
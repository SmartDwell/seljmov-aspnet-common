using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Seljmov.AspNet.Commons.Options;

namespace Seljmov.AspNet.Commons.Helpers;

/// <summary>
/// Jwt reader.
/// </summary>
/// <remarks>[Ru] Читатель Jwt.</remarks>
public class JwtReader
{
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly ILogger<JwtReader> _logger;

    /// <summary>
    /// Constructor for <see cref="JwtReader"/>.
    /// </summary>
    /// <remarks>[Ru] Конструктор для <see cref="JwtReader"/>.</remarks>
    /// <param name="jwtOptions">Jwt options.</param>
    /// <param name="logger">Logger.</param>
    public JwtReader(IOptions<JwtOptions> jwtOptions, ILogger<JwtReader> logger)
    {
        _jwtOptions = jwtOptions;
        _logger = logger;
    }

    /// <summary>
    /// Read access token.
    /// </summary>
    /// <remarks>[Ru] Прочитать токен доступа.</remarks>
    /// <param name="token">Jwt token.</param>
    /// <param name="claims">Claims.</param>
    /// <param name="validTo">Valid to.</param>
    /// <returns>True if token is valid, otherwise false.</returns>
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
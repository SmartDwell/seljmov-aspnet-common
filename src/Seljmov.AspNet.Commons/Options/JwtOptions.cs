using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Seljmov.AspNet.Commons.Options;

/// <summary>
/// JWT options.
/// </summary>
/// <remarks>[Ru] Опции JWT.</remarks>
public class JwtOptions
{
    /// <summary>
    /// Issuer of token.
    /// </summary>
    /// <remarks>[Ru] Издатель токена.</remarks>
    public string Issuer { get; set; } = string.Empty;
    
    /// <summary>
    /// Audience of token.
    /// </summary>
    /// <remarks>[Ru] Потребитель токена.</remarks>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Key for token.
    /// </summary>
    /// <remarks>[Ru] Ключ для токена.</remarks>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Access token lifetime (in minutes).
    /// </summary>
    /// <remarks>[Ru] Время жизни токена доступа (в минутах).</remarks>
    public int AccessTokenLifetime { get; set; }
    
    /// <summary>
    /// Refresh token lifetime (in minutes).
    /// </summary>
    /// <remarks>[Ru] Время жизни токена обновления (в минутах).</remarks>
    public int RefreshTokenLifetime { get; set; }
    
    /// <summary>
    /// Get symmetric security key.
    /// </summary>
    /// <remarks>[Ru] Получить симметричный ключ безопасности.</remarks>
    /// <returns>Symmetric security key.</returns>
    public SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
}
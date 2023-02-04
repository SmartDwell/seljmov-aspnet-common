using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Seljmov.AspNet.Commons.Options;

/// <summary>
/// Настройки JWT-токена
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Издатель токена
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    
    /// <summary>
    /// Потребитель токена
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Ключ шифрования
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Время жизни токена доступа
    /// </summary>
    /// <remarks>Время в минутах</remarks>
    public int AccessTokenLifetime { get; set; }
    
    /// <summary>
    /// Время жизни токена обновления
    /// </summary>
    /// <remarks>Время в минутах</remarks>
    public int RefreshTokenLifetime { get; set; }
    
    /// <summary>
    /// Получить симметричный ключ шифрования
    /// </summary>
    public SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
}
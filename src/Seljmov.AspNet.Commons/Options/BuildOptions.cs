using System.Collections.Generic;

namespace Seljmov.AspNet.Commons.Options;

/// <summary>
/// Build options for web application.
/// </summary>
/// <remarks>[Ru] Опции сборки для веб-приложения.</remarks>
public class BuildOptions
{
    /// <summary>
    /// Use JWT authentication. If true, you need to add <see cref="JwtOptions"/> to configuration.
    /// </summary>
    /// <remarks>[Ru] Использовать JWT-аутентификацию. Если true, необходимо добавить <see cref="JwtOptions"/> в конфигурацию.</remarks>
    public bool UseJwtAuthentication { get; set; } = true;
    
    /// <summary>
    /// Use CORS.
    /// </summary>
    /// <remarks>[Ru] Использовать CORS.</remarks>
    public bool UseCors { get; set; }

    /// <summary>
    /// Authentication policies.
    /// </summary>
    /// <remarks>[Ru] Политики аутентификации.</remarks>
    public IReadOnlyCollection<string> AuthenticationPolicies { get; set; } = [];
}

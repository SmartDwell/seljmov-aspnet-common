namespace Seljmov.AspNet.Commons.Options;

/// <summary>
/// Опции сборки.
/// </summary>
public class BuildOptions
{
    /// <summary>
    /// Использовать JWT-авторизацию.
    /// </summary>
    public bool UseJwtAuthentication { get; set; } = true;
    
    /// <summary>
    /// Использовать CORS.
    /// </summary>
    public bool UseCors { get; set; } = true;
}

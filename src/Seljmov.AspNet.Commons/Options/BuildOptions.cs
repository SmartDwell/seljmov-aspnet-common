namespace Seljmov.AspNet.Commons.Options;

/// <summary>
/// Опции билда
/// </summary>
public class BuildOptions
{
    /// <summary>
    /// Использовать JWT-авторизацию
    /// </summary>
    public bool UseJwtAuthentication { get; set; } = true;
}
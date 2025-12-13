namespace WinGetConfigApplier.Models;

/// <summary>
/// Represents the root configuration object
/// </summary>
public class AppConfiguration
{
    public List<Application> Applications { get; set; } = new();
    public ConfigurationSettings Configuration { get; set; } = new();
}

/// <summary>
/// Represents an application to be installed
/// </summary>
public class Application
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = "winget";
    public InstallOptions InstallOptions { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents installation options for an application
/// </summary>
public class InstallOptions
{
    public bool Silent { get; set; } = true;
    public bool AcceptPackageAgreements { get; set; } = true;
}

/// <summary>
/// Represents global configuration settings
/// </summary>
public class ConfigurationSettings
{
    public string MinOsVersion { get; set; } = "10.0.22631";
    public bool EnableDeveloperMode { get; set; } = false;
    public string LogLevel { get; set; } = "Information";
}

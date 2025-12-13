using System.Text;
using WinGetConfigApplier.Models;

namespace WinGetConfigApplier.Services;

/// <summary>
/// Builds a WinGet YAML configuration from the JSON configuration
/// </summary>
public class WinGetConfigurationBuilder
{
    private readonly ILogger _logger;

    public WinGetConfigurationBuilder(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates a WinGet configuration YAML file from the application configuration
    /// </summary>
    public string BuildConfiguration(AppConfiguration config, string outputPath)
    {
        try
        {
            _logger.LogInformation("Building WinGet configuration YAML...");
            
            var yaml = new StringBuilder();
            
            // Header
            yaml.AppendLine("# yaml-language-server: $schema=https://aka.ms/configuration-dsc-schema/0.2");
            yaml.AppendLine("properties:");
            
            // Assertions section
            yaml.AppendLine("  assertions:");
            yaml.AppendLine("    - resource: Microsoft.Windows.Developer/OsVersion");
            yaml.AppendLine("      directives:");
            yaml.AppendLine("        description: Verify min OS version requirement");
            yaml.AppendLine("        allowPrerelease: false");
            yaml.AppendLine("      settings:");
            yaml.AppendLine($"        MinVersion: '{config.Configuration.MinOsVersion}'");
            yaml.AppendLine();
            
            // Resources section
            yaml.AppendLine("  resources:");
            yaml.AppendLine();
            
            // Developer Mode (if enabled)
            if (config.Configuration.EnableDeveloperMode)
            {
                yaml.AppendLine("    - resource: Microsoft.Windows.Developer/DeveloperMode");
                yaml.AppendLine("      directives:");
                yaml.AppendLine("        description: Enable Developer Mode");
                yaml.AppendLine("        allowPrerelease: false");
                yaml.AppendLine("      settings:");
                yaml.AppendLine("        Ensure: Present");
                yaml.AppendLine();
            }
            
            // Applications
            foreach (var app in config.Applications)
            {
                yaml.AppendLine("    - resource: Microsoft.WinGet.DSC/WinGetPackage");
                yaml.AppendLine($"      id: {SanitizeId(app.Name)}");
                yaml.AppendLine("      directives:");
                yaml.AppendLine($"        description: {app.Description}");
                yaml.AppendLine("        allowPrerelease: false");
                yaml.AppendLine("      settings:");
                yaml.AppendLine($"        id: {app.Id}");
                yaml.AppendLine($"        source: {app.Source}");
                yaml.AppendLine();
            }
            
            // Footer
            yaml.AppendLine("  configurationVersion: 0.2.0");
            
            // Write to file
            File.WriteAllText(outputPath, yaml.ToString());
            _logger.LogInformation($"WinGet configuration written to: {outputPath}");
            
            return outputPath;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to build WinGet configuration: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Sanitizes an ID to be valid YAML identifier
    /// </summary>
    private string SanitizeId(string name)
    {
        // Remove spaces and special characters, convert to camelCase
        var sanitized = new StringBuilder();
        bool capitalizeNext = false;
        
        foreach (char c in name)
        {
            if (char.IsLetterOrDigit(c))
            {
                if (capitalizeNext)
                {
                    sanitized.Append(char.ToUpper(c));
                    capitalizeNext = false;
                }
                else if (sanitized.Length == 0)
                {
                    sanitized.Append(char.ToLower(c));
                }
                else
                {
                    sanitized.Append(c);
                }
            }
            else
            {
                capitalizeNext = true;
            }
        }
        
        return sanitized.ToString() + "Package";
    }
}

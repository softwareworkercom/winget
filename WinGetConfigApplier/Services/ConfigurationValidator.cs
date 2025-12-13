using WinGetConfigApplier.Models;

namespace WinGetConfigApplier.Services;

/// <summary>
/// Validates the application configuration
/// </summary>
public class ConfigurationValidator
{
    private readonly ILogger _logger;

    public ConfigurationValidator(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Validates the configuration and returns validation errors
    /// </summary>
    public List<string> Validate(AppConfiguration config)
    {
        var errors = new List<string>();

        if (config == null)
        {
            errors.Add("Configuration is null");
            return errors;
        }

        if (config.Applications == null || config.Applications.Count == 0)
        {
            errors.Add("No applications specified in configuration");
        }
        else
        {
            for (int i = 0; i < config.Applications.Count; i++)
            {
                var app = config.Applications[i];
                
                if (string.IsNullOrWhiteSpace(app.Id))
                {
                    errors.Add($"Application at index {i}: Id is required");
                }
                
                if (string.IsNullOrWhiteSpace(app.Name))
                {
                    errors.Add($"Application at index {i}: Name is required");
                }
                
                if (string.IsNullOrWhiteSpace(app.Source))
                {
                    errors.Add($"Application at index {i}: Source is required");
                }
            }
        }

        if (config.Configuration != null)
        {
            if (string.IsNullOrWhiteSpace(config.Configuration.MinOsVersion))
            {
                errors.Add("MinOsVersion is required in configuration settings");
            }
        }

        if (errors.Count > 0)
        {
            _logger.LogError($"Configuration validation failed with {errors.Count} error(s)");
            foreach (var error in errors)
            {
                _logger.LogError($"  - {error}");
            }
        }
        else
        {
            _logger.LogInformation("Configuration validation passed");
        }

        return errors;
    }
}

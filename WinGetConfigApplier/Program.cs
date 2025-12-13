using System.Text.Json;
using WinGetConfigApplier.Models;
using WinGetConfigApplier.Services;

namespace WinGetConfigApplier;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("=== WinGet Configuration Applier ===");
        Console.WriteLine();

        // Parse command line arguments
        string configPath = args.Length > 0 ? args[0] : "apps-config.json";
        string outputPath = args.Length > 1 ? args[1] : "generated-winget-config.yaml";

        // Display usage if help is requested
        if (args.Length > 0 && (args[0] == "--help" || args[0] == "-h"))
        {
            DisplayHelp();
            return 0;
        }

        try
        {
            // Check if config file exists
            if (!File.Exists(configPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Configuration file not found: {configPath}");
                Console.ResetColor();
                Console.WriteLine();
                DisplayHelp();
                return 1;
            }

            Console.WriteLine($"Configuration file: {configPath}");
            Console.WriteLine($"Output file: {outputPath}");
            Console.WriteLine();

            // Read and parse JSON configuration
            var jsonContent = await File.ReadAllTextAsync(configPath);
            var config = JsonSerializer.Deserialize<AppConfiguration>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Failed to parse configuration file");
                Console.ResetColor();
                return 1;
            }

            // Initialize services
            var logger = new ConsoleLogger(config.Configuration.LogLevel);
            var validator = new ConfigurationValidator(logger);
            var builder = new WinGetConfigurationBuilder(logger);
            var installer = new WinGetInstaller(logger);

            // Validate configuration
            logger.LogInformation("Validating configuration...");
            var validationErrors = validator.Validate(config);
            
            if (validationErrors.Count > 0)
            {
                logger.LogError($"Configuration validation failed with {validationErrors.Count} error(s)");
                return 1;
            }

            // Check if WinGet is available
            logger.LogInformation("Checking if WinGet is available...");
            bool wingetAvailable = await installer.IsWinGetAvailableAsync();
            
            if (!wingetAvailable)
            {
                logger.LogWarning("WinGet is not available on this system");
                logger.LogInformation("Configuration will be generated but not applied");
            }

            // Build WinGet configuration
            var configFilePath = builder.BuildConfiguration(config, outputPath);

            // Display summary
            logger.LogInformation("=== Configuration Summary ===");
            logger.LogInformation($"Total applications to install: {config.Applications.Count}");
            foreach (var app in config.Applications)
            {
                logger.LogInformation($"  - {app.Name} ({app.Id})");
            }
            Console.WriteLine();

            // Skip installation if WinGet is not available
            if (!wingetAvailable)
            {
                logger.LogInformation($"Configuration saved to: {outputPath}");
                logger.LogInformation("To apply this configuration on a Windows system with WinGet, run:");
                logger.LogInformation($"  winget configure -f \"{outputPath}\" --accept-configuration-agreements");
                return 0;
            }

            // Ask for confirmation
            Console.Write("Do you want to apply this configuration? (y/N): ");
            var confirmation = Console.ReadLine()?.Trim().ToLower();

            if (confirmation != "y" && confirmation != "yes")
            {
                logger.LogInformation("Operation cancelled by user");
                logger.LogInformation($"Configuration saved to: {outputPath}");
                return 0;
            }

            // Apply configuration
            logger.LogInformation("Starting installation...");
            var result = await installer.ApplyConfigurationAsync(configFilePath);

            if (result.Success)
            {
                logger.LogInformation("=== Installation completed successfully ===");
                return 0;
            }
            else
            {
                logger.LogError("=== Installation failed ===");
                logger.LogError(result.ErrorMessage);
                return 1;
            }
        }
        catch (JsonException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error parsing JSON configuration: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unexpected error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
            return 1;
        }
    }

    static void DisplayHelp()
    {
        Console.WriteLine("Usage: WinGetConfigApplier [config-file] [output-file]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  config-file    Path to JSON configuration file (default: apps-config.json)");
        Console.WriteLine("  output-file    Path to output YAML file (default: generated-winget-config.yaml)");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --help, -h     Display this help message");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  WinGetConfigApplier apps-config.json output.yaml");
        Console.WriteLine();
    }
}

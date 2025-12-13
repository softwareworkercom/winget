using System.Diagnostics;
using WinGetConfigApplier.Models;

namespace WinGetConfigApplier.Services;

/// <summary>
/// Executes WinGet commands to install applications
/// </summary>
public class WinGetInstaller
{
    private readonly ILogger _logger;

    public WinGetInstaller(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Applies a WinGet configuration file
    /// </summary>
    public async Task<InstallationResult> ApplyConfigurationAsync(string configFilePath)
    {
        var result = new InstallationResult();
        
        try
        {
            _logger.LogInformation($"Applying WinGet configuration: {configFilePath}");
            
            if (!File.Exists(configFilePath))
            {
                result.Success = false;
                result.ErrorMessage = $"Configuration file not found: {configFilePath}";
                _logger.LogError(result.ErrorMessage);
                return result;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = $"configure -f \"{configFilePath}\" --accept-configuration-agreements",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _logger.LogInformation($"Executing: winget {startInfo.Arguments}");

            using var process = new Process { StartInfo = startInfo };
            
            var outputBuilder = new List<string>();
            var errorBuilder = new List<string>();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.Add(e.Data);
                    _logger.LogInformation($"  {e.Data}");
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorBuilder.Add(e.Data);
                    _logger.LogWarning($"  {e.Data}");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            result.ExitCode = process.ExitCode;
            result.Output = string.Join(Environment.NewLine, outputBuilder);
            result.ErrorOutput = string.Join(Environment.NewLine, errorBuilder);

            if (process.ExitCode == 0)
            {
                result.Success = true;
                _logger.LogInformation("WinGet configuration applied successfully");
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = $"WinGet configure exited with code {process.ExitCode}";
                _logger.LogError(result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Exception during WinGet configuration: {ex.Message}";
            _logger.LogError(result.ErrorMessage);
        }

        return result;
    }

    /// <summary>
    /// Checks if WinGet is available on the system
    /// </summary>
    public async Task<bool> IsWinGetAvailableAsync()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                _logger.LogInformation($"WinGet version: {output.Trim()}");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"WinGet is not available: {ex.Message}");
            return false;
        }
    }
}

/// <summary>
/// Represents the result of an installation operation
/// </summary>
public class InstallationResult
{
    public bool Success { get; set; }
    public int ExitCode { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

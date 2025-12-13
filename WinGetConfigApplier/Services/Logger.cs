namespace WinGetConfigApplier.Services;

/// <summary>
/// Simple logging interface
/// </summary>
public interface ILogger
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogDebug(string message);
}

/// <summary>
/// Console-based logger implementation
/// </summary>
public class ConsoleLogger : ILogger
{
    private readonly string _logLevel;

    public ConsoleLogger(string logLevel = "Information")
    {
        _logLevel = logLevel;
    }

    public void LogInformation(string message)
    {
        if (ShouldLog("Information"))
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }
    }

    public void LogWarning(string message)
    {
        if (ShouldLog("Warning"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }
    }

    public void LogError(string message)
    {
        if (ShouldLog("Error"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }
    }

    public void LogDebug(string message)
    {
        if (ShouldLog("Debug"))
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[DEBUG] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }
    }

    private bool ShouldLog(string level)
    {
        var levels = new Dictionary<string, int>
        {
            { "Debug", 0 },
            { "Information", 1 },
            { "Warning", 2 },
            { "Error", 3 }
        };

        return levels.TryGetValue(level, out var currentLevel) &&
               levels.TryGetValue(_logLevel, out var configuredLevel) &&
               currentLevel >= configuredLevel;
    }
}

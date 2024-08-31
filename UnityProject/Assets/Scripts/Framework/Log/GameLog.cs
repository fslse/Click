using Cysharp.Text;
using Microsoft.Extensions.Logging;
using ZLogger;
using ZLogger.Providers;
using ZLogger.Unity;

namespace Framework.Log;

public static class GameLog
{
    private static readonly ILogger<Log> logger = LoggerFactory.Create(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Trace);
        logging.AddZLoggerUnityDebug(); // log to UnityDebug

        // Add to output to the file
        logging.AddZLoggerFile("Logs/Runtime/GameLog.log", options => { options.UseJsonFormatter(); });

        // Add to output the file that rotates at constant intervals.
        logging.AddZLoggerRollingFile(options =>
            {
                // File name determined by parameters to be rotated
                options.FilePathSelector = (timestamp, sequenceNumber) =>
                    $"Logs/Runtime/GameLog_{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";

                // The period of time for which you want to rotate files at time intervals.
                options.RollingInterval = RollingInterval.Day;

                // Limit of size if you want to rotate by file size. (KB)
                options.RollingSizeKB = 1024;

                options.UseJsonFormatter();
            }
        );
    }).CreateLogger<Log>();

    public static ILogger<Log> Logger => logger;

    public static void LogDebug(string message)
    {
        logger.LogDebug(message);
    }

    public static void LogDebug(string key, string value)
    {
        using var message = ZString.CreateStringBuilder();
        message.Append(key);
        message.Append(" : ");
        message.Append(value);
        logger.LogDebug(message.ToString());
    }

    public static void LogDebug(string? info, params string[] strings)
    {
        using var message = ZString.CreateStringBuilder();
        if (info != null) message.Append(info);
        foreach (var str in strings)
        {
            message.Append("\n");
            message.Append(str);
        }

        logger.LogDebug(message.ToString());
    }

    public static void LogWarning(string message)
    {
        logger.LogWarning(message);
    }

    public static void LogWarning(string key, string value)
    {
        using var message = ZString.CreateStringBuilder();
        message.Append(key);
        message.Append(" : ");
        message.Append(value);
        logger.LogWarning(message.ToString());
    }

    public static void LogWarning(string? info, params string[] strings)
    {
        using var message = ZString.CreateStringBuilder();
        if (info != null) message.Append(info);
        foreach (var str in strings)
        {
            message.Append("\n");
            message.Append(str);
        }

        logger.LogWarning(message.ToString());
    }

    public static void LogError(string message)
    {
        logger.LogError(message);
    }

    public static void LogError(string key, string value)
    {
        using var message = ZString.CreateStringBuilder();
        message.Append(key);
        message.Append(" : ");
        message.Append(value);
        logger.LogError(message.ToString());
    }

    public static void LogError(string? info, params string[] strings)
    {
        using var message = ZString.CreateStringBuilder();
        if (info != null) message.Append(info);
        foreach (var str in strings)
        {
            message.Append("\n");
            message.Append(str);
        }

        logger.LogError(message.ToString());
    }
}

public abstract class Log
{
}
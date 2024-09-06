using Microsoft.Extensions.Logging;
using Scripts.Framework.Manager;
using UnityEngine;
using ZLogger;
using ZLogger.Providers;
using ZLogger.Unity;

namespace Scripts.Framework.Log;

public static class GameLog
{
#if UNITY_EDITOR
    private const string logPath = "Logs/Runtime/";
#else
    private static readonly string logPath = Application.persistentDataPath + "/Logs/Runtime/";
#endif

    public abstract class Log
    {
    }

    public static ILogger<Log> Logger { get; } = LoggerFactory.Create(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Trace);
        logging.AddZLoggerUnityDebug(); // log to UnityDebug
    }).CreateLogger<Log>();

    private static ILogger<Log> RuntimeLogger { get; } = LoggerFactory.Create(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Trace);

        // Add to output to the file
        logging.AddZLoggerFile(logPath + "GameLog.log", options => { options.UseJsonFormatter(); });

        // Add to output the file that rotates at constant intervals.
        logging.AddZLoggerRollingFile(options =>
            {
                // File name determined by parameters to be rotated
                options.FilePathSelector = (timestamp, sequenceNumber) =>
                    $"{logPath}GameLog_{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";

                // The period of time for which you want to rotate files at time intervals.
                options.RollingInterval = RollingInterval.Day;

                // Limit of size if you want to rotate by file size. (KB)
                options.RollingSizeKB = 1024;

                options.UseJsonFormatter();
            }
        );
    }).CreateLogger<Log>();

    public static void LogDebug(string message)
    {
        Logger.ZLogDebug($"message: {message}");
    }

    public static void LogDebug(string key, string value)
    {
        Logger.ZLogDebug($"{key} : {value}");
    }

    public static void LogWarning(string message)
    {
        Logger.ZLogWarning($"message: {message}");
    }

    public static void LogWarning(string key, string value)
    {
        Logger.ZLogWarning($"{key} : {value}");
    }

    public static void LogError(string message)
    {
        Logger.ZLogError($"message: {message}");
    }

    public static void LogError(string key, string value)
    {
        Logger.ZLogError($"{key} : {value}");
    }

    public static void HandleLog(string logString, string stackTrace, LogType type)
    {
        RuntimeLogger.ZLogTrace($"{type}\n{logString}\n{stackTrace}");
    }
}
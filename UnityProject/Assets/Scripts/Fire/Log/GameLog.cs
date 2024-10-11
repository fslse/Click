using System.Diagnostics;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ZLogger.Providers;
using ZLogger.Unity;

namespace Scripts.Fire.Log
{
    public static class GameLog
    {
#if UNITY_EDITOR
        private const string LogPath = "Logs/Runtime/";
#else
        private static readonly string LogPath = Application.persistentDataPath + "/Logs/Runtime/";
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
            logging.AddZLoggerFile(LogPath + "GameLog.log", options => { options.UseJsonFormatter(); });

            // Add to output the file that rotates at constant intervals.
            logging.AddZLoggerRollingFile(options =>
                {
                    // File name determined by parameters to be rotated
                    options.FilePathSelector = (timestamp, sequenceNumber) =>
                        $"{LogPath}GameLog_{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";

                    // The period of time for which you want to rotate files at time intervals.
                    options.RollingInterval = RollingInterval.Day;

                    // Limit of size if you want to rotate by file size. (KB)
                    options.RollingSizeKB = 1024;

                    options.UseJsonFormatter();
                }
            );
        }).CreateLogger<Log>();

        [Conditional("VERSION_DEV")]
        public static void LogDebug(string message)
        {
            message = message.Replace("\n", "</b></color>\n<color=white><b>");
            Logger.ZLogDebug($"<color=white><b>[GameLog {Time.frameCount}] \u25ba - {message}</b></color>");
        }

        [Conditional("VERSION_DEV")]
        public static void LogDebug(string key, string value)
        {
            key = key.Replace("\n", "</b></color>\n<color=white><b>");
            value = value.Replace("\n", "</b></color>\n<color=white><b>");
            Logger.ZLogDebug($"<color=white><b>[GameLog {Time.frameCount}] \u25ba - {key}: {value}</b></color>");
        }

        [Conditional("VERSION_DEV")]
        public static void LogWarning(string message)
        {
            message = message.Replace("\n", "</b></color>\n<color=yellow><b>");
            Logger.ZLogWarning($"<color=yellow><b>[GameLog {Time.frameCount}] \u25ba - {message}</b></color>");
        }

        [Conditional("VERSION_DEV")]
        public static void LogWarning(string key, string value)
        {
            key = key.Replace("\n", "</b></color>\n<color=yellow><b>");
            value = value.Replace("\n", "</b></color>\n<color=yellow><b>");
            Logger.ZLogWarning($"<color=yellow><b>[GameLog {Time.frameCount}] \u25ba - {key}: {value}</b></color>");
        }

        [Conditional("VERSION_DEV")]
        public static void LogError(string message)
        {
            message = message.Replace("\n", "</b></color>\n<color=red><b>");
            Logger.ZLogError($"<color=red><b>[GameLog {Time.frameCount}] \u25ba - {message}</b></color>");
        }

        [Conditional("VERSION_DEV")]
        public static void LogError(string key, string value)
        {
            key = key.Replace("\n", "</b></color>\n<color=red><b>");
            value = value.Replace("\n", "</b></color>\n<color=red><b>");
            Logger.ZLogError($"<color=red><b>[GameLog {Time.frameCount}] \u25ba - {key}: {value}</b></color>");
        }

        public static void HandleLog(string logString, string stackTrace, LogType type)
        {
#if VERSION_DEV
            RuntimeLogger.ZLogTrace($"{type}\n{logString}\n{stackTrace}");
#endif
        }
    }
}
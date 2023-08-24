using System.Diagnostics;
using BepInEx.Logging;

namespace LessZdoCorruption;

internal class Log
{
    internal static ManualLogSource? Logger;

    [Conditional("Debug")]
    public static void DevConditionally(bool condition, string message)
    {
        if (condition)
        {
            DevelopmentOnly(message);
        }
    }

    [Conditional("Debug")]
    public static void DevConditionally(Func<bool> condition, string message)
    {
        if (condition())
        {
            DevelopmentOnly(message);
        }
    }

    [Conditional("Debug")]
    public static void DevelopmentOnly(string message) => Logger!.LogWarning($"{message}");

    public static void Debug(string message) => Logger!.LogInfo($"{message}");

    public static void Trace(string message) => Logger!.LogDebug($"{message}");
    
    public static void Info(string message) => Logger!.LogMessage($"{message}");

    public static void Warning(string message) => Logger!.LogWarning($"{message}");

    public static void Warning(string message, Exception e) => Logger!.LogWarning($"{message}\n{e}");

    public static void Error(string message) => Logger!.LogError($"{message}");

    public static void Error(string message, Exception e) => Logger!.LogError($"{message}\n{e}");
}
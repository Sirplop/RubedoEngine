using System;

namespace Rubedo;

/// <summary>
/// TODO: I am Log, and I don't have a summary yet.
/// </summary>
public class Log
{
    /// <summary>
    /// The NLog backend. You probably don't need to bother with this unless you're doing setup.
    /// </summary>
    public static NLog.Logger Logger { get; set; }

    /// <summary>
    /// Logs info to the log file.
    /// </summary>
    /// <param name="msg">The message to log</param>
    public static void Info(string msg) => Logger.Info(msg);
    /// <summary>
    /// Logs an exception to the log file. Why would you log an exception as info?
    /// </summary>
    /// <param name="e">The exception to log</param>
    /// <param name="msg">The message to log</param>
    public static void Info(Exception e, string msg) => Logger.Info(e, msg);

    /// <summary>
    /// Logs info to the console. This will not do anything in Release builds.
    /// </summary>
    /// <param name="msg">The message to log</param>
    public static void Debug(string msg) {
#if DEBUG
        Logger.Debug(msg);
#endif
    }
    /// <summary>
    /// Logs an exception to the log file. This will not do anything in Release builds.
    /// </summary>
    /// <param name="e">The exception to log</param>
    /// <param name="msg">The message to log</param>
    public static void Debug(Exception e, string msg) {
#if DEBUG
        Logger.Debug(e, msg);
#endif
    }

    /// <summary>
    /// Logs a warning to the console.
    /// </summary>
    /// <param name="msg">The message to log</param>
    public static void Warn(string msg) => Logger.Warn(msg);
    /// <summary>
    /// Logs an exception to the log file as a warning.
    /// </summary>
    /// <param name="e">The exception to log</param>
    /// <param name="msg">The message to log</param>
    public static void Warn(Exception e, string msg) => Logger.Warn(e, msg);

    /// <summary>
    /// Logs an error to the console.
    /// </summary>
    /// <param name="msg">The message to log</param>
    public static void Error(string msg) => Logger.Error(msg);
    /// <summary>
    /// Logs an exception to the log file as an error.
    /// </summary>
    /// <param name="e">The exception to log</param>
    /// <param name="msg">The message to log</param>
    public static void Error(Exception e, string msg) => Logger.Error(e, msg);

    /// <summary>
    /// Logs a fatal error to the console. The program should probably crash or something after this.
    /// </summary>
    /// <param name="msg">The message to log</param>
    public static void Fatal(string msg) => Logger.Fatal(msg);
    /// <summary>
    /// Logs a fatal exception to the console. The program should probably crash or something after this.
    /// </summary>
    /// <param name="e">The exception to log</param>
    /// <param name="msg">The message to log</param>
    public static void Fatal(Exception e, string msg) => Logger.Fatal(e, msg);
}
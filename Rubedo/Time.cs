using Microsoft.Xna.Framework;
using System;

namespace Rubedo;

/// <summary>
/// Static time class, updated by the main game class.
/// </summary>
public static class Time
{
    /// <summary>
    /// Elapsed time since last frame, in seconds, scaled by the <see cref="TimeScale"/>
    /// </summary>
    public static float DeltaTime => deltaTime;
    /// <summary>
    /// The elapsed time since last frame, in milliseconds, scaled by the <see cref="TimeScale"/>
    /// </summary>
    public static float DeltaTimeMillis => deltaTimeMillis;
    /// <summary>
    /// <see cref="DeltaTime"/> that has not been scaled by the <see cref="TimeScale"/>.
    /// </summary>
    public static double RawDeltaTime => rawDeltaTime;
    /// <summary>
    /// <see cref="DeltaTimeMillis"/> that has not been scaled by the <see cref="TimeScale"/>.
    /// </summary>
    public static double RawDeltaTimeMillis => rawDeltaTimeMillis;
    /// <summary>
    /// The total time the game has been running for, in seconds.
    /// </summary>
    public static double RunningTime => rawTime;
    /// <summary>
    /// A scale applied to <see cref="DeltaTime"/>.
    /// </summary>
    public static float TimeScale => timeScale;

    /// <summary>
    /// The update rate for fixed updates. Defaults to 50 times per second.
    /// </summary>
    public static float FixedDeltaTime => fixedDeltaTime;

    private static float deltaTime;
    private static float deltaTimeMillis;
    private static double rawDeltaTime;
    private static double rawDeltaTimeMillis;
    private static double rawTime;
    private static float timeScale = 1.0f;
    private static float fixedDeltaTime = 0.02f;

    internal static void UpdateTime(GameTime gameTime)
    {
        rawTime = gameTime.TotalGameTime.TotalSeconds;
        rawDeltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        deltaTime = (float)rawDeltaTime * timeScale;
        rawDeltaTimeMillis = gameTime.ElapsedGameTime.TotalMilliseconds;
        deltaTimeMillis = (float)rawDeltaTimeMillis * timeScale;
    }
    /// <summary>
    /// Sets the <see cref="TimeScale"/>. Will clamp negative values to 0.
    /// </summary>
    /// <param name="scale"></param>
    public static void SetTimeScale(float scale)
    {
        scale = MathF.Max(scale, 0);
        timeScale = scale;
    }

    /// <summary>
    /// Sets the fixed update rate.
    /// </summary>
    public static void SetFixedDeltaTime(float time)
    {
        fixedDeltaTime = time;
    }
}
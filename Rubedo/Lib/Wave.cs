using System;

namespace Rubedo.Lib;

/// <summary>
/// Provides a variety of different continuous wave patterns.
/// </summary>
public static class Wave
{
    /// <summary>
    /// Like <see cref="Sawtooth(in float, in float)"/> but only goes negative to positive - upon reaching 1, it will reset to -1.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="period"></param>
    /// <remarks>Looks like  /|/|/|</remarks>
    /// <returns></returns>
    public static float ZeroToOneSawtooth(in float time, in float period)
    {
        float tP = time / period;
        return tP - MathF.Floor(tP);
    }
    /// <summary>
    /// Alternates between moving from -1 and 1, and back.
    /// </summary>
    /// <param name="time">The time position of the wave.</param>
    /// <param name="period">How often the wave repeats.</param>
    /// <remarks>Looks like /\/\/\/</remarks>
    /// <returns></returns>
    public static float Sawtooth(in float time, in float period)
    {
        float tP = time / period;
        return 2 * (tP - MathF.Floor(0.5f - tP));
    }
    /// <summary>
    /// Like <see cref="Sawtooth(in float, in float)"/> but stays positive.
    /// </summary>
    /// <param name="time">The time position of the wave.</param>
    /// <param name="period">How often the wave repeats.</param>
    /// <remarks>Looks like ^^^^</remarks>
    /// <returns></returns>
    public static float Triangle(in float time, in float period)
    {
        return MathF.Abs(Sawtooth(time, period));
    }
    /// <summary>
    /// A sine wave.
    /// </summary>
    /// <param name="time">The time position of the wave.</param>
    /// <param name="period">How often the wave repeats.</param>
    /// <param name="amplitude">Height of the wave.</param>
    /// <param name="offset">Offsets the time by the given amount.</param>
    /// <returns></returns>
    public static float Sine(in float time, in float period, in float amplitude, in float offset)
    {
        return amplitude * MathF.Sin((2 * MathF.PI * time / period) + offset);
    }
    /// <summary>
    /// A cosine wave.
    /// </summary>
    /// <param name="time">The time position of the wave.</param>
    /// <param name="period">How often the wave repeats.</param>
    /// <param name="amplitude">Height of the wave.</param>
    /// <param name="offset">Offsets the time by the given amount.</param>
    /// <returns></returns>
    public static float Cosine(in float time, in float period, in float amplitude, in float offset)
    {
        return amplitude * MathF.Cos((2 * MathF.PI * time / period) + offset);
    }
    /// <summary>
    /// A <see cref="Sine(in float, in float, in float, in float)"/> wave but taking the sign of the function, so it flips between -1 and 1.
    /// </summary>
    /// <param name="time">The time position of the wave.</param>
    /// <param name="period">How often the wave repeats.</param>
    /// <param name="offset">Offsets the time by the given amount.</param>
    /// <returns></returns>
    public static float Square(in float time, in float period, in float offset)
    {
        return MathF.Sign(Sine(time, period, 1, offset));
    }
}
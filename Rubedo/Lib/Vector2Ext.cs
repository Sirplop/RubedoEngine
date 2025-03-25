using System.Runtime.CompilerServices;
using System;
using Microsoft.Xna.Framework;

namespace Rubedo.Lib;

/// <summary>
/// I am Vector2Ext, and this is my summary.
/// </summary>
public static class Vector2Ext
{
    /// <summary>
    /// Workaround to Vector2.Normalize screwing up the [0,0] vector
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref Vector2 vec)
    {
        var magnitude = MathF.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));
        if (magnitude > Math.EPSILON)
            vec /= magnitude;
        else
            vec.X = vec.Y = 0;
    }
    /// <summary>
    /// Workaround to Vector2.Normalize screwing up the [0,0] vector
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalize(Vector2 vec)
    {
        var magnitude = MathF.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));
        if (magnitude > Math.EPSILON)
            vec /= magnitude;
        else
            vec.X = vec.Y = 0;

        return vec;
    }

    /// <summary>
    /// returns the vector perpendicular to the passed in vectors
    /// </summary>
    /// <param name="first">First.</param>
    /// <param name="second">Second.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Perpendicular(ref Vector2 first, ref Vector2 second)
    {
        return new Vector2(-1f * (second.Y - first.Y), second.X - first.X);
    }

    /// <summary>
    /// returns the vector perpendicular to the passed in vectors
    /// </summary>
    /// <param name="first">First.</param>
    /// <param name="second">Second.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Perpendicular(Vector2 first, Vector2 second)
    {
        return new Vector2(-1f * (second.Y - first.Y), second.X - first.X);
    }

    /// <summary>
    /// flips the x/y values and inverts the y to get the perpendicular
    /// </summary>
    /// <param name="original">Original.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Perpendicular(Vector2 original)
    {
        return new Vector2(-original.Y, original.X);
    }
}
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Lib;

public static class Math
{
    public const float EPSILON = 0.0005f;
    public const float DEG2RAD = 0.0174532924f;
    public const float RAD2DEG = 57.29578f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(float val)
    {
        return (int)MathF.Floor(val);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilToInt(float val)
    {
        return (int)MathF.Ceiling(val);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(in float val, in float min, in float max)
    {
        if (val < min)
            return min;
        if (val > max)
            return max;
        return val;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int val, int min, int max)
    {
        if (val < min)
            return min;
        if (val > max)
            return max;
        return val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(in Vector2 vec)
    {
        return MathF.Max(vec.X, vec.Y);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(in Vector2 vec)
    {
        return MathF.Min(vec.X, vec.Y);
    }
    public static bool NearlyEqual(float a, float b)
    {
        return MathF.Abs(a - b) < EPSILON;
    }
    public static bool NearlyEqual(Vector2 a, Vector2 b)
    {
        return Vector2.DistanceSquared(a, b) <= EPSILON;
    }
    public static bool NearlyEqual(ref Vector2 a, ref Vector2 b)
    {
        Vector2.DistanceSquared(ref a, ref b, out float val);
        return val <= EPSILON;
    }
    public static bool NearlyZero(ref Vector2 a)
    {
        return a.LengthSquared() <= EPSILON;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToRadians(float angleDegrees)
    {
        return DEG2RAD * angleDegrees;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToDegrees(float angleRadians)
    {
        return RAD2DEG * angleRadians;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool BiasGreaterThan(float a, float b)
    {
        const float BiasRelative = 0.95f;
        const float BiasAbsolute = 0.01f;
        return a >= b * BiasRelative + a * BiasAbsolute;
    }


    /// <summary>
    /// Gets the value T from a lerp, using the inputs <paramref name="a"/> and <paramref name="b"/> and the output <paramref name="c"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float LerpTVal(float a, float b, float c)
    {
        if (a == b) //prevent divide by 0
            return 1;
        return (c - a) / (b - a);
    }
}
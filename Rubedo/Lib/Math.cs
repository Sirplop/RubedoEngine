using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Lib;

public static class Math
{
    public const float EPSILON = 0.0005f; //suitably small float value to say "yeah they're close enough". NOTE: this does not work for very large float values.
    public const float DEG2RAD = 0.0174532924f;
    public const float RAD2DEG = 57.29578f;

    /// <summary>
    /// Floors the value, then casts it to an int.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(float val)
    {
        return (int)MathF.Floor(val);
    }
    /// <summary>
    /// Ceilings the value, then casts it to an int.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilToInt(float val)
    {
        return (int)MathF.Ceiling(val);
    }
    /// <summary>
    /// Rounds the value, then casts it to an int.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(float val)
    {
        return (int)MathF.Round(val);
    }
    /// <summary>
    /// Clamps the value between <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <remarks> Does not check if min is less than max. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(in float val, in float min, in float max)
    {
        if (val < min)
            return min;
        if (val > max)
            return max;
        return val;
    }
    /// <summary>
    /// Clamps the value between <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <remarks> Does not check if min is less than max. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int val, int min, int max)
    {
        if (val < min)
            return min;
        if (val > max)
            return max;
        return val;
    }
    /// <summary>
    /// Clamps the value between 0 and 1.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp01(float v)
    {
        return Clamp(v, 0.0f, 1.0f);
    }
    /// <summary>
    /// Gets the largest component of the given vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(in Vector2 vec)
    {
        return MathF.Max(vec.X, vec.Y);
    }
    /// <summary>
    /// Gets the smallest component of the given vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(in Vector2 vec)
    {
        return MathF.Min(vec.X, vec.Y);
    }

    /// <summary>
    /// Gets the minimum value of the provided values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(in float v1, in float v2)
    {
        return MathF.Min(v1, v2);
    }
    /// <summary>
    /// Gets the minimum value of the provided values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(in float v1, in float v2, in float v3)
    {
        return MathF.Min(v3, MathF.Min(v1, v2));
    }
    /// <summary>
    /// Gets the minimum value of the provided values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(in float v1, in float v2, in float v3, in float v4)
    {
        return MathF.Min(MathF.Min(v1, v2), MathF.Min(v3, v4));
    }

    /// <summary>
    /// Gets the maximum value of the provided values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(in float v1, in float v2)
    {
        return MathF.Max(v1, v2);
    }
    /// <summary>
    /// Gets the maximum value of the provided values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(in float v1, in float v2, in float v3)
    {
        return MathF.Max(v3, MathF.Max(v1, v2));
    }
    /// <summary>
    /// Gets the maximum value of the provided values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(in float v1, in float v2, in float v3, in float v4)
    {
        return MathF.Max(MathF.Max(v1, v2), MathF.Max(v3, v4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NearlyEqual(float a, float b)
    {
        return MathF.Abs(a - b) < EPSILON;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NearlyEqual(Vector2 a, Vector2 b)
    {
        return Vector2.DistanceSquared(a, b) <= EPSILON;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NearlyEqual(ref Vector2 a, ref Vector2 b)
    {
        Vector2.DistanceSquared(ref a, ref b, out float val);
        return val <= EPSILON;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// Interpolates between <paramref name="a"/> and <paramref name="b"/> by <paramref name="weightB"/>.
    /// </summary>
    /// <remarks> Behaviour undefined if <paramref name="weightB"/> is outside the range 0..1 </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Mix(float a, float b, float weightB)
    {
        //also written as   a + (b - a) * t = c
        return ((1 - weightB) * a) + (weightB * b);
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
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
        return MathF.Max(MathF.Min(val, max), min);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int val, int min, int max)
    {
        return System.Math.Max(System.Math.Min(val, max), min);
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

    /// <summary>
    /// Rotates the given point around (0,0) by the specified degree angle. This is a local-space transform.
    /// </summary>
    public static Vector2 Rotate(in Vector2 v, float delta)
    {
        delta = ToRadians(delta);
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        return new Vector2(
            v.X * cos - v.Y * sin,
            v.X * sin + v.Y * cos
        );
    }
    /// <summary>
    /// Rotates the given point around (0,0) by the specified degree angle. This is a local-space transform.
    /// </summary>
    public static void Rotate(float x, float y, float delta, out float z, out float w)
    {
        delta = ToRadians(delta);
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        z = x * cos - y * sin;
        w = x * sin + y * cos;
    }
    /// <summary>
    /// Rotates the given point around (0,0) by the specified radian angle. This is a local-space transform.
    /// </summary>
    public static Vector2 RotateRadians(in Vector2 v, float delta, float scaleX = 1, float scaleY = 1)
    {
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        return new Vector2(
            v.X * (cos * scaleX) - v.Y * (sin * scaleY),
            v.X * (sin * scaleX) + v.Y * (cos * scaleY)
        );
    }
    /// <summary>
    /// Rotates the given point around (0,0) by the specified degree angle. This is a local-space transform.
    /// </summary>
    public static void RotateRadians(float x, float y, float delta, out float z, out float w)
    {
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        z = x * cos - y * sin;
        w = x * sin + y * cos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool BiasGreaterThan(float a, float b)
    {
        const float BiasRelative = 0.95f;
        const float BiasAbsolute = 0.01f;
        return a >= b * BiasRelative + a * BiasAbsolute;
    }
}
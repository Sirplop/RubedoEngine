using Microsoft.Xna.Framework;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rubedo.Lib;

public static class Math
{
    public const float EPSILON = 1e-6f;
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
    public static float Clamp(float val, float min, float max)
    {
        return MathF.Max(MathF.Min(val, max), min);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int val, int min, int max)
    {
        return System.Math.Max(System.Math.Min(val, max), min);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(Vector2 vec)
    {
        return MathF.Max(vec.X, vec.Y);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(Vector2 vec)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DegToRad(float angleDegrees)
    {
        return DEG2RAD * angleDegrees;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RadToDeg(float angleRadians)
    {
        return RAD2DEG * angleRadians;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalize(Vector2 value)
    {
        if (value == Vector2.Zero)
            return Vector2.Zero;
        float invLen = 1f / MathF.Sqrt(value.X * value.X + value.Y * value.Y);
        return new Vector2(value.X * invLen, value.Y * invLen);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref float x, ref float y)
    {
        if (x == 0 && y == 0)
        {
            x = 0;
            y = 0;
            return;
        }
        float invLen = 1f / MathF.Sqrt(x * x + y * y);
        x *= invLen;
        y *= invLen;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalize(float x, float y)
    {
        if (x == 0 && y == 0)
            return Vector2.Zero;
        float invLen = 1f / MathF.Sqrt(x * x + y * y);
        return new Vector2(x * invLen, y * invLen);
    }

    /// <summary>
    /// Computes the cross product of vectors A and B.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cross(in Vector2 a, in Vector2 b)
    {
        // cz = ax * by − ay * bx
        return a.X * b.Y - a.Y * b.X;
    }

    /// <summary>
    /// Rotates the given point around (0,0) by the specified degree angle. This is a local-space transform.
    /// </summary>
    public static Vector2 Rotate(in Vector2 v, float delta)
    {
        delta = DegToRad(delta);
        return new Vector2(
            v.X * MathF.Cos(delta) - v.Y * MathF.Sin(delta),
            v.X * MathF.Sin(delta) + v.Y * MathF.Cos(delta)
        );
    }
    /// <summary>
    /// Rotates the given point around (0,0) by the specified degree angle. This is a local-space transform.
    /// </summary>
    public static void Rotate(float x, float y, float delta, out float z, out float w)
    {
        delta = DegToRad(delta);
        z = x * MathF.Cos(delta) - y * MathF.Sin(delta);
        w = x * MathF.Sin(delta) + y * MathF.Cos(delta);
    }
    /// <summary>
    /// Rotates the given point around (0,0) by the specified degree angle. This is a local-space transform.
    /// </summary>
    public static void RotateRadians(float x, float y, float delta, out float z, out float w)
    {
        z = x * MathF.Cos(delta) - y * MathF.Sin(delta);
        w = x * MathF.Sin(delta) + y * MathF.Cos(delta);
    }
    /// <summary>
    /// Rotates the given point around (0,0) by the specified radian angle. This is a local-space transform.
    /// </summary>
    public static Vector2 RotateRadians(in Vector2 v, float delta)
    {
        return new Vector2(
            v.X * MathF.Cos(delta) - v.Y * MathF.Sin(delta),
            v.X * MathF.Sin(delta) + v.Y * MathF.Cos(delta)
        );
    }

    #region Wave Functions
    public static class Wave
    {
        public static float ZeroToOneSawtooth(in float time, in float period)
        {
            return time / period - (MathF.Floor(time / period));
        }
        public static float Sawtooth(in float time, in float period)
        {
            return 2 * ((time / period) - MathF.Floor(0.5f - time / period));
        }
        public static float Triangle(in float time, in float period)
        {
            return MathF.Abs(Sawtooth(time, period));
        }
        public static float Sine(in float time, in float period, in float amplitude, in float offset)
        {
            return amplitude * MathF.Sin((2 * MathF.PI * time / period) + offset);
        }
        public static float Cosine(in float time, in float period, in float amplitude, in float offset)
        {
            return amplitude * MathF.Cos((2 * MathF.PI * time / period) + offset);
        }
        public static float Square(in float time, in float period, in float amplitude, in float offset)
        {
            return MathF.Sign(Sine(time, period, amplitude, offset));
        }
    }
    #endregion
}

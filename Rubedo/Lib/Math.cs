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
    public static void Normalize(ref Vector2 vec)
    {
        if (NearlyEqual(vec, Vector2.Zero))
        {
            vec.X = vec.Y = 0;
            return;
        }
        var mag = 1 / MathF.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));
        vec.X *= mag;
        vec.Y *= mag;
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
        return (a.X * b.Y) - (a.Y * b.X);
    }
    /// <summary>
    /// Rotate a vector 90 degrees counter-clockwise.
    /// </summary>
    /// <returns>A vector perpendicular to vector <paramref name="a"/> and the Z axis</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Left(in Vector2 a, in float z = 1)
    {
        return new Vector2(-a.Y * z, a.X * z);
    }
    /// <summary>
    /// Rotate a vector 90 degrees clockwise.
    /// </summary>
    /// <returns>A vector perpendicular to vector <paramref name="a"/> and the Z axis</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Right(in Vector2 a, in float z = 1)
    {
        return new Vector2(a.Y * z, -a.X * z);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool BiasGreaterThan(float a, float b)
    {
        const float BiasRelative = 0.95f;
        const float BiasAbsolute = 0.01f;
        return a >= b * BiasRelative + a * BiasAbsolute;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Square(in float x)
    {
        return x * x;
    }

    public static Vector2 Rotate(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X * b.X - a.Y * b.Y, a.Y * b.X + a.X * b.Y);
    }

    public static Vector2 InvRotate(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X * b.X + a.Y * b.Y, a.Y * b.X - a.X * b.Y);
    }

    /// <summary>
    /// A + S * B
    /// </summary>
    public static Vector2 MulAdd(Vector2 A, float S, Vector2 B)
    {
        return new Vector2(A.X + S * B.X, A.Y + S * B.Y);
    }
    /// <summary>
    /// A - S * B
    /// </summary>
    public static Vector2 MulSub(Vector2 A, float S, Vector2 B)
    {
        return new Vector2(A.X - S * B.X, A.Y - S * B.Y);
    }

    /// <summary>
    /// Converts an HSV color value to RGB.
    /// h (hue) should be in [0,360), s (saturation) and v (value) in [0,1].
    /// </summary>
    public static void HsvToRgb(double h, double s, double v, out int r, out int g, out int b)
    {
        double c = v * s;
        double x = c * (1 - System.Math.Abs((h / 60) % 2 - 1));
        double m = v - c;
        double r1, g1, b1;

        if (h < 60)
        {
            r1 = c; g1 = x; b1 = 0;
        }
        else if (h < 120)
        {
            r1 = x; g1 = c; b1 = 0;
        }
        else if (h < 180)
        {
            r1 = 0; g1 = c; b1 = x;
        }
        else if (h < 240)
        {
            r1 = 0; g1 = x; b1 = c;
        }
        else if (h < 300)
        {
            r1 = x; g1 = 0; b1 = c;
        }
        else
        {
            r1 = c; g1 = 0; b1 = x;
        }

        r = (int)((r1 + m) * 255);
        g = (int)((g1 + m) * 255);
        b = (int)((b1 + m) * 255);
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

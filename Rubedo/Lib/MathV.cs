using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Lib;

/// <summary>
/// Vector math library.
/// </summary>
public static class MathV
{
    /// <summary>
    /// Probably dumb, but who cares.
    /// </summary>
    public static bool FastEquals(ref Vector2 a, ref Vector2 b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    /// <summary>
    /// Adds (<paramref name="b"/> * <paramref name="c"/>) to <paramref name="a"/>, and returns the result in <paramref name="d"/>.
    /// </summary>
    public static void MulAdd(ref Vector2 a, ref Vector2 b, float c, out Vector2 d)
    {
        d.X = a.X + (b.X * c);
        d.Y = a.Y + (b.Y * c);
    }
    /// <summary>
    /// Subtracts (<paramref name="b"/> * <paramref name="c"/>) from <paramref name="a"/>, and returns the result in <paramref name="d"/>.
    /// </summary>
    public static void MulSub(ref Vector2 a, ref Vector2 b, float c, out Vector2 d)
    {
        d.X = a.X - (b.X * c);
        d.Y = a.Y - (b.Y * c);
    }
    /// <summary>
    /// (<paramref name="a"/> * <paramref name="aScale"/>) + (<paramref name="b"/> * <paramref name="bScale"/>).
    /// </summary>
    /// <param name="a">The vector a.</param>
    /// <param name="aScale">The value <paramref name="a"/> is multiplied by.</param>
    /// <param name="b">The vector b.</param>
    /// <param name="bScale">The value <paramref name="b"/> is multiplied by.</param>
    public static void MulAdd2(ref Vector2 a, float aScale, ref Vector2 b, float bScale, out Vector2 c)
    {
        c.X = (a.X * aScale) + (b.X * bScale);
        c.Y = (a.Y * aScale) + (b.Y * bScale);
    }
    /// <summary>
    /// (<paramref name="a"/> * <paramref name="aScale"/>) - (<paramref name="b"/> * <paramref name="bScale"/>).
    /// </summary>
    /// <param name="a">The vector a.</param>
    /// <param name="aScale">The value <paramref name="a"/> is multiplied by.</param>
    /// <param name="b">The vector b.</param>
    /// <param name="bScale">The value <paramref name="b"/> is multiplied by.</param>
    public static void MulSub2(ref Vector2 a, float aScale, ref Vector2 b, float bScale, out Vector2 c)
    {
        c.X = (a.X * aScale) + (b.X * bScale);
        c.Y = (a.Y * aScale) + (b.Y * bScale);
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
    /// Computes the cross product of vectors A and B.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Cross(ref Vector2 a, ref Vector2 b, out float l)
    {
        l = (a.X * b.Y) - (a.Y * b.X);
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
    /// Rotate a vector 90 degrees counter-clockwise.
    /// </summary>
    /// <returns>A vector perpendicular to vector <paramref name="a"/> and the Z axis</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Left(ref Vector2 a, out Vector2 left)
    {
        float aX = a.X; //must copy a.X in case a is left.
        left.X = -a.Y;
        left.Y = aX;
    }
    /// <summary>
    /// Rotate a vector 90 degrees counter-clockwise.
    /// </summary>
    /// <returns>A vector perpendicular to vector <paramref name="a"/> and the Z axis</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Left(ref Vector2 a, in float z, out Vector2 left)
    {
        float aX = a.X;
        left.X = -a.Y * z;
        left.Y = aX * z;
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
    /// Rotate a vector 90 degrees clockwise.
    /// </summary>
    /// <returns>A vector perpendicular to vector <paramref name="a"/> and the Z axis</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Right(ref Vector2 a, out Vector2 right)
    {
        float aX = a.X;
        right.X = a.Y;
        right.Y = -aX;
    }
    /// <summary>
    /// Rotate a vector 90 degrees clockwise.
    /// </summary>
    /// <returns>A vector perpendicular to vector <paramref name="a"/> and the Z axis</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Right(ref Vector2 a, in float z, out Vector2 right)
    {
        float aX = a.X;
        right.X = a.Y * z;
        right.Y = -aX * z;
    }

    /// <summary>
    /// Scales the given vector to unit length.
    /// </summary>
    /// <param name="vec">The vector to be normalized.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalize(Vector2 vec)
    {
        if (vec == Vector2.Zero)
            return Vector2.Zero;
        float invLen = 1f / MathF.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        return new Vector2(vec.X * invLen, vec.Y * invLen);
    }
    /// <summary>
    /// Scales the given vector to unit length.
    /// </summary>
    /// <param name="vec">The vector to be normalized.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref Vector2 vec)
    {
        if (Math.NearlyZero(ref vec))
        {
            vec.X = vec.Y = 0;
            return;
        }
        var mag = 1 / MathF.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));
        vec.X *= mag;
        vec.Y *= mag;
    }

    /// <summary>
    /// Scales the given vector components to unit length.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
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
    /// <summary>
    /// Scales the given vector components to unit length.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalize(float x, float y)
    {
        if (x == 0 && y == 0)
            return Vector2.Zero;
        float invLen = 1f / MathF.Sqrt(x * x + y * y);
        return new Vector2(x * invLen, y * invLen);
    }

    /// <summary>
    /// Rotates <paramref name="v"/> by <paramref name="delta"/> degrees.
    /// </summary>
    public static Vector2 Rotate(in Vector2 v, float delta)
    {
        delta = Math.ToRadians(delta);
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        return new Vector2(
            v.X * cos - v.Y * sin,
            v.X * sin + v.Y * cos
        );
    }
    /// <summary>
    /// Rotates <paramref name="v"/> by <paramref name="delta"/> degrees.
    /// </summary>
    public static void Rotate(ref Vector2 v, float delta, out Vector2 o)
    {
        delta = Math.ToRadians(delta);
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        o.X = v.X * cos - v.Y * sin;
        o.Y = v.X * sin + v.Y * cos;
    }
    /// <summary>
    /// Rotates the vector (<paramref name="x"/>, <paramref name="y"/>) by <paramref name="delta"/> degrees.
    /// </summary>
    public static void Rotate(float x, float y, float delta, out float z, out float w)
    {
        delta = Math.ToRadians(delta);
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        z = x * cos - y * sin;
        w = x * sin + y * cos;
    }
    /// <summary>
    /// Rotates <paramref name="v"/> by <paramref name="delta"/> radians.
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
    /// Rotates <paramref name="v"/> by <paramref name="delta"/> radians.
    /// </summary>
    public static void RotateRadians(ref Vector2 v, float delta, out Vector2 o)
    {
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        o.X = v.X * cos - v.Y * sin;
        o.Y = v.X * sin + v.Y * cos;
    }
    /// <summary>
    /// Rotates the vector (<paramref name="x"/>, <paramref name="y"/>) by <paramref name="delta"/> radians.
    /// </summary>
    public static void RotateRadians(float x, float y, float delta, out float z, out float w)
    {
        float sin = MathF.Sin(delta);
        float cos = MathF.Cos(delta);
        z = x * cos - y * sin;
        w = x * sin + y * cos;
    }
}
using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Lib;

/// <summary>
/// A represnetation of a float 2x3 matrix containing position, rotation, and scale.
/// </summary>
public struct Matrix2D
{
    public float M11; // x scale
    public float M12;

    public float M21;
    public float M22; // y scale

    public float M31; // x translation
    public float M32; // y translation

    /// <summary>
    /// Returns the identity matrix.
    /// </summary>
    public static Matrix2D Identity => _identity;
    static Matrix2D _identity = new Matrix2D(1f, 0f, 0f, 1f, 0f, 0f);

    #region Constructors
    /// <summary>
    /// Constructs a matrix.
    /// </summary>
    public Matrix2D(float m11, float m12, float m21, float m22, float m31, float m32)
    {
        M11 = m11;
        M12 = m12;

        M21 = m21;
        M22 = m22;

        M31 = m31;
        M32 = m32;
    }
    public static Matrix2D CreateTranslation(float x, float y)
    {
        return new Matrix2D(1, 0, 0, 1, x, y);
    }
    public static Matrix2D CreateTranslation(Vector2 xy)
    {
        return new Matrix2D(1, 0, 0, 1, xy.X, xy.Y);
    }
    public static void CreateTranslation(float x, float y, out Matrix2D matrix)
    {
        matrix.M11 = 1; matrix.M12 = 0;
        matrix.M21 = 0; matrix.M22 = 1;
        matrix.M31 = x; matrix.M32 = y;
    }
    public static void CreateTranslation(Vector2 xy, out Matrix2D matrix)
    {
        matrix.M11 = 1;    matrix.M12 = 0;
        matrix.M21 = 0;    matrix.M22 = 1;
        matrix.M31 = xy.X; matrix.M32 = xy.Y;
    }
    public static Matrix2D CreateRotation(float radians)
    {
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);
        return new Matrix2D(cos, sin, -sin, cos, 0, 0);
    }
    public static void CreateRotation(float radians, out Matrix2D matrix)
    {
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);

        matrix.M11 = cos;  matrix.M12 = sin;
        matrix.M21 = -sin; matrix.M22 = cos;
        matrix.M31 = 0;    matrix.M32 = 0;
    }
    public static Matrix2D CreateScale(float x, float y)
    {
        return new Matrix2D(x, 0, 0, y, 0, 0);
    }
    public static Matrix2D CreateScale(Vector2 xy)
    {
        return new Matrix2D(xy.X, 0, 0, xy.Y, 0, 0);
    }
    public static void CreateScale(float x, float y, out Matrix2D matrix)
    {
        matrix.M11 = x; matrix.M12 = 0;
        matrix.M21 = 0; matrix.M22 = y;
        matrix.M31 = 0; matrix.M32 = 0;
    }
    public static void CreateScale(Vector2 xy, out Matrix2D matrix)
    {
        matrix.M11 = xy.X; matrix.M12 = 0;
        matrix.M21 = 0; matrix.M22 = xy.Y;
        matrix.M31 = 0; matrix.M32 = 0;
    }
    public static Matrix2D CreateTR(float x, float y, float radians)
    {
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);
        return new Matrix2D(cos, sin, -sin, cos, x, y);
    }
    public static void CreateTR(float x, float y, float radians, out Matrix2D matrix)
    {
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);

        matrix.M11 = cos;  matrix.M12 = sin;
        matrix.M21 = -sin; matrix.M22 = cos;
        matrix.M31 = x;    matrix.M32 = y;
    }
    public static Matrix2D CreateRS(float radians, float x, float y)
    {
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);

        float cosScaleX = cos * x;
        float sinScaleX = sin * x;
        float cosScaleY = cos * y;
        float sinScaleY = sin * y;

        return new Matrix2D(cosScaleX, sinScaleX, -sinScaleY, cosScaleY, 0, 0);
    }
    public static void CreateRS(float radians, float x, float y, out Matrix2D matrix)
    {
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);

        float cosScaleX = cos * x;
        float sinScaleX = sin * x;
        float cosScaleY = cos * y;
        float sinScaleY = sin * y;

        matrix.M11 = cosScaleX;  matrix.M12 = sinScaleX;
        matrix.M21 = -sinScaleY; matrix.M22 = cosScaleY;
        matrix.M31 = x;          matrix.M32 = 0;
    }
    public static Matrix2D CreateRS(float radians, Vector2 xy)
    {
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);

        float cosScaleX = cos * xy.X;
        float sinScaleX = sin * xy.X;
        float cosScaleY = cos * xy.Y;
        float sinScaleY = sin * xy.Y;

        return new Matrix2D(cosScaleX, sinScaleX, -sinScaleY, cosScaleY, 0, 0);
    }
    public static void CreateRS(float radians, Vector2 xy, out Matrix2D matrix)
    {
        CreateRS(radians, xy.X, xy.Y, out matrix);
    }
    public static Matrix2D CreateTS(float x, float y, float sX, float sY)
    {
        return new Matrix2D(sX, 0, 0, sY, x, y);
    }
    public static void CreateTS(float x, float y, float sX, float sY, out Matrix2D matrix)
    {
        matrix.M11 = sX; matrix.M12 = 0;
        matrix.M21 = 0; matrix.M22 = sY;
        matrix.M31 = x; matrix.M32 = y;
    }
    #endregion


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Determinant()
    {
        return M11 * M22 - M12 * M21;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invert(ref Matrix2D matrix, out Matrix2D result)
    {
        var det = 1 / matrix.Determinant();

        result.M11 = matrix.M22 * det;
        result.M12 = -matrix.M12 * det;

        result.M21 = -matrix.M21 * det;
        result.M22 = matrix.M11 * det;

        result.M31 = (matrix.M32 * matrix.M21 - matrix.M31 * matrix.M22) * det;
        result.M32 = -(matrix.M32 * matrix.M11 - matrix.M31 * matrix.M12) * det;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the multiplication of two matrices.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(ref Matrix2D matrix1, ref Matrix2D matrix2, out Matrix2D result)
    {
        float m11 = (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21);
        float m12 = (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22);

        float m21 = (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21);
        float m22 = (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22);

        float m31 = (matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21) + matrix2.M31;
        float m32 = (matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22) + matrix2.M32;

        result.M11 = m11; result.M12 = m12;
        result.M21 = m21; result.M22 = m22;
        result.M31 = m31; result.M32 = m32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 TransformPoint(in Vector2 value)
    {
        return TransformPoint(value.X, value.Y);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 TransformPoint(in float X, in float Y)
    {
        return new Vector2
        {
            //this is the form (rotationscale * current offset) + matrix position.
            X = (M11 * X + M21 * Y) + M31,
            Y = (M12 * X + M22 * Y) + M32
        };
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TransformPoint(in Vector2 value, out Vector2 result)
    {
        result.X = (M11 * value.X + M21 * value.Y) + M31;
        result.Y = (M12 * value.X + M22 * value.Y) + M32;
    }

    public void SetPosition(ref Vector2 val)
    {
        M31 = val.X; M32 = val.Y;
    }
}
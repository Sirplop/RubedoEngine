using Microsoft.Xna.Framework;
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
    public static Matrix2D CreateRotation(float rotationDegrees)
    {
        float sin = MathF.Sin(Lib.Math.DegToRad(rotationDegrees));
        float cos = MathF.Cos(Lib.Math.DegToRad(rotationDegrees));
        return new Matrix2D(cos, sin, -sin, cos, 0, 0);
    }
    public static Matrix2D CreateScale(float x, float y)
    {
        return new Matrix2D(x, 0, 0, y, 0, 0);
    }
    public static Matrix2D CreateTR(float x, float y, float rotationDegrees)
    {
        float sin = MathF.Sin(Lib.Math.DegToRad(rotationDegrees));
        float cos = MathF.Cos(Lib.Math.DegToRad(rotationDegrees));
        return new Matrix2D(cos, sin, -sin, cos, x, y);
    }
    public static Matrix2D CreateRS(float rotationDegrees, float x, float y)
    {
        float sin = MathF.Sin(Lib.Math.DegToRad(rotationDegrees));
        float cos = MathF.Cos(Lib.Math.DegToRad(rotationDegrees));

        float cosScaleX = cos * x;
        float sinScaleX = sin * x;
        float cosScaleY = cos * y;
        float sinScaleY = sin * y;

        return new Matrix2D(cosScaleX, sinScaleX, -sinScaleY, cosScaleY, 0, 0);
    }
    public static Matrix2D CreateTS(float x, float y, float sX, float sY)
    {
        return new Matrix2D(sX, 0, 0, sY, x, y);
    }



    /// <summary>
    /// Creates a new matrix which contains the sum of the two matrixes.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add(ref Matrix2D matrix1, ref Matrix2D matrix2, out Matrix2D result)
    {
        result.M11 = matrix1.M11 + matrix2.M11;
        result.M12 = matrix1.M12 + matrix2.M12;

        result.M21 = matrix1.M21 + matrix2.M21;
        result.M22 = matrix1.M22 + matrix2.M22;

        result.M31 = matrix1.M31 + matrix2.M31;
        result.M32 = matrix1.M32 + matrix2.M32;
    }

    public Vector2 Transform(in Vector2 value)
    {
        return Transform(value.X, value.Y);
    }
    public Vector2 Transform(in float X, in float Y)
    {
        return new Vector2
        {
            //this is the form (rotationscale * current offset) + matrix position.
            X = (M11 * X + M21 * Y) + M31,
            Y = (M12 * X + M22 * Y) + M32
        };
    }
}
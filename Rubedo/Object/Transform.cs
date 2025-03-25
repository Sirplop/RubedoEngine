
using Microsoft.Xna.Framework;
using Rubedo.Lib;
using System;
using System.Runtime.InteropServices;
using static System.Formats.Asn1.AsnWriter;

namespace Rubedo.Object;

public class Transform : IEquatable<Transform>
{
    public readonly static Transform Identity = new Transform(Vector2.Zero, 0, Vector2.One);

    public Vector2 Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
        }
    }
    private Vector2 _position;
    public float Rotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            _rotation = value % 360;
        }
    }
    public float RotationRadians
    {
        get
        {
            return Lib.Math.DegToRad(_rotation);
        }
        set
        {
            _rotation = Lib.Math.RadToDeg(value) % 360;
        }
    }
    private float _rotation;
    public Vector2 Scale
    {
        get
        {
            return _scale;
        }
        set
        {
            _scale = value;
        }
    }
    private Vector2 _scale;

    public Transform()
    {
        _position = Vector2.Zero;
        _rotation = 0;
        _scale = Vector2.One;
    }
    public Transform(Vector2 position)
    {
        _position = position;
        _rotation = 0;
        _scale = Vector2.One;
    }
    public Transform(Vector2 position, float rotation)
    {
        _position = position;
        _rotation = rotation % 360;
        _scale = Vector2.One;
    }
    public Transform(Vector2 position, float rotation, Vector2 scale)
    {
        _position = position;
        _rotation = rotation % 360;
        _scale = scale;
    }

    public Matrix2D ToMatrix()
    {
        float sin = MathF.Sin(Lib.Math.DegToRad(_rotation));
        float cos = MathF.Cos(Lib.Math.DegToRad(_rotation));

        float cosScaleX = cos * _scale.X;
        float sinScaleX = sin * _scale.X;
        float cosScaleY = cos * _scale.Y;
        float sinScaleY = sin * _scale.Y;

        return new Matrix2D(cosScaleX, sinScaleX, -sinScaleY, cosScaleY, _position.X, _position.Y);
    }

    public bool Equals(Transform other)
    {
        return _position.Equals(other._position) && _rotation.Equals(other._rotation) && _scale.Equals(other._scale);
    }

    public static Transform operator +(Transform left, Transform right) 
    {
        return new Transform(left._position + right._position, left._rotation + right._rotation, left._scale * right._scale);
    }
    public static Transform operator -(Transform left, Transform right)
    {
        return new Transform(left._position - right._position, left._rotation - right._rotation, left._scale / right._scale);
    }
}
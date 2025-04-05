
using Microsoft.Xna.Framework;
using Rubedo.Lib;
using System;
using System.Runtime.InteropServices;
using static System.Formats.Asn1.AsnWriter;

namespace Rubedo.Object;

public class Transform : IEquatable<Transform>
{
    public readonly static Transform Identity = new Transform(Vector2.Zero, 0, Vector2.One);

    public Transform parent;
    #region Local Properties
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
    public float RotationDegrees
    {
        get
        {
            return Lib.Math.RadToDeg(_rotation);
        }
        set
        {
            _rotation = Lib.Math.RadToDeg(value % 360);
        }
    }
    public float Rotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            _rotation = value % MathHelper.TwoPi;
        }
    }
    //rotation is stored in radians.
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
    #endregion
    #region World Properties
    public Vector2 WorldPosition
    {
        get
        {
            if (parent == null)
                return _position;
            return parent.ToMatrixWorld().Transform(_position);
        }
    }
    /// <summary>
    /// In radians.
    /// </summary>
    public float WorldRotation
    {
        get
        {
            if (parent == null)
                return _rotation;
            return parent.WorldRotation + _rotation;
        }
    }
    public float WorldRotationDegrees
    {
        get
        {
            if (parent == null)
                return Lib.Math.RadToDeg(_rotation);
            return Lib.Math.RadToDeg(parent.WorldRotation + _rotation);
        }
    }
    public Vector2 WorldScale
    {
        get
        {
            if (parent == null)
                return _scale;
            return parent.WorldScale * _scale;
        }
    }
    #endregion
    #region Constructors
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
    #endregion

    /// <summary>
    /// Constructs a Matrix version of this transform, ignoring parent transforms.
    /// </summary>
    /// <returns></returns>
    public Matrix2D ToMatrixLocal()
    {
        float sin = MathF.Sin(_rotation);
        float cos = MathF.Cos(_rotation);

        float cosScaleX = cos * _scale.X;
        float sinScaleX = sin * _scale.X;
        float cosScaleY = cos * _scale.Y;
        float sinScaleY = sin * _scale.Y;

        return new Matrix2D(cosScaleX, sinScaleX, -sinScaleY, cosScaleY, _position.X, _position.Y);
    }
    /// <summary>
    /// Constructs a Matrix version of this transform, including any parent transforms.
    /// </summary>
    /// <returns></returns>
    public Matrix2D ToMatrixWorld()
    {
        if (parent == null)
            return ToMatrixLocal();
        float rot = WorldRotation;
        float sin = MathF.Sin(rot);
        float cos = MathF.Cos(rot);

        Vector2 scale = WorldScale;
        float cosScaleX = cos * scale.X;
        float sinScaleX = sin * scale.X;
        float cosScaleY = cos * scale.Y;
        float sinScaleY = sin * scale.Y;
        Vector2 pos = WorldPosition;

        return new Matrix2D(cosScaleX, sinScaleX, -sinScaleY, cosScaleY, pos.X, pos.Y);
    }

    public bool Equals(Transform other)
    {
        return _position.Equals(other._position) && _rotation.Equals(other._rotation) && _scale.Equals(other._scale);
    }
    //TODO: Make use a cached matrix.
    public Vector2 WorldToLocalPosition(Vector2 worldPosition)
    {
        return Lib.Math.RotateRadians(worldPosition - WorldPosition, -WorldRotation);
    }
    public Vector2 LocalToWorldPosition(Vector2 localPosition)
    {
        return Lib.Math.RotateRadians(localPosition + WorldPosition, WorldRotation);
    }
    public Vector2 WorldToLocalDirection(Vector2 worldDirection)
    {
        return Lib.Math.RotateRadians(worldDirection, -WorldRotation);
    }
    public Vector2 LocalToWorldDirection(Vector2 localDirection)
    {
        return Lib.Math.RotateRadians(localDirection, WorldRotation);
    }
}
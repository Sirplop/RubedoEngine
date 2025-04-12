using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo.Lib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rubedo.Object;

/// <summary>
/// Contains information about a given object's position, rotation, and scale in space, possibly relative to a parent.
/// </summary>
public class Transform
{
    [Flags]
    enum Dirt
    {
        Clean = 0,
        Position = 1,
        Rotation = 2,
        Scale = 4
    }

    private  Transform _parent;
    private readonly List<Transform> _children = new List<Transform>();

    private Dirt _dirtyType = (Dirt)7; //all dirty at start.
    private bool _localDirty = true;

    private bool _positionDirty = true;
    private bool _worldPositionDirty = true;
    private bool _rotationDirty = true;
    private bool _scaleDirty = true;
    private bool _worldToLocalDirty = true;

    private Vector2 _position;
    private float _rotation;
    private Vector2 _scale;

    private Vector2 _localPosition;
    private float _localRotation; //rotation is stored in radians.
    private Vector2 _localScale;

    private Matrix2D _localTransform;
    private Matrix2D _worldTransform = Matrix2D.Identity;
    private Matrix2D _worldToLocalTransform = Matrix2D.Identity;

    private Matrix2D _positionMatrix;
    private Matrix2D _rotationMatrix;
    private Matrix2D _scaleMatrix;

    public int ChildCount => _children.Count;
    public Transform Parent
    {
        get => _parent;
        set => SetParent(value);
    }

    #region Constructors
    public Transform()
    {
        _localPosition = Vector2.Zero;
        _localRotation = 0;
        _localScale = Vector2.One;
    }
    public Transform(Vector2 position)
    {
        _localPosition = position;
        _localRotation = 0;
        _localScale = Vector2.One;
    }
    public Transform(Vector2 position, float rotationDegrees)
    {
        _localPosition = position;
        _localRotation = Lib.Math.DegToRad(rotationDegrees % 360);
        _localScale = Vector2.One;
    }
    public Transform(Vector2 position, float rotationDegrees, Vector2 scale)
    {
        _localPosition = position;
        _localRotation = Lib.Math.DegToRad(rotationDegrees % 360);
        _localScale = scale;
    }
    #endregion

    #region Local
    /// <summary>
    /// Gets the local-space position.
    /// </summary>
    public Vector2 LocalPosition
    {
        get
        {
            UpdateTransform();
            return _localPosition;
        }
        set => SetLocalPosition(value);
    }
    /// <summary>
    /// Gets the local-space rotation in degrees.
    /// </summary>
    public float LocalRotationDegrees
    {
        get
        {
            UpdateTransform();
            return Lib.Math.RadToDeg(_localRotation);
        }
        set => SetLocalRotation(Lib.Math.DegToRad(value));
    }
    /// <summary>
    /// Gets the local-space rotation in radians.
    /// </summary>
    public float LocalRotation
    {
        get
        {
            UpdateTransform();
            return _localRotation;
        }
        set => SetLocalRotation(value);
    }
    /// <summary>
    /// Gets the local-space scale.
    /// </summary>
    public Vector2 LocalScale
    {
        get
        {
            UpdateTransform();
            return _localScale;
        }
        set => SetLocalScale(value);
    }
    #endregion
    #region World
    /// <summary>
    /// Gets the world-space position.
    /// </summary>
    public Vector2 Position
    {
        get
        {
            UpdateTransform();
            if (_worldPositionDirty)
            {
                if (_parent != null)
                {
                    _parent.UpdateTransform();
                    _parent._worldTransform.TransformPoint(_localPosition, out _position);
                }
                else
                {
                    _position = _localPosition;
                }

                _worldPositionDirty = false;
            }

            return _position;
        }
        set => SetPosition(value);
    }
    /// <summary>
    /// Get the world-space rotation in degrees.
    /// </summary>
    public float RotationDegrees
    {
        get
        {
            UpdateTransform();
            return Lib.Math.RadToDeg(_rotation);
        }
        set => SetRotationDegrees(value);
    }
    /// <summary>
    /// Gets the world-space rotation in radians.
    /// </summary>
    public float Rotation
    {
        get
        {
            UpdateTransform();
            return _rotation;
        }
        set => SetRotation(value);
    }
    /// <summary>
    /// Gets the world-space scale.
    /// </summary>
    public Vector2 Scale
    {
        get
        {
            UpdateTransform();
            return _scale;
        }
        set => SetScale(value);
    }
    #endregion
    #region Matrix
    public Matrix2D LocalToWorldTransform
    {
        get
        {
            UpdateTransform();
            return _worldTransform;
        }
    }


    public Matrix2D WorldToLocalTransform
    {
        get
        {
            if (_worldToLocalDirty)
            {
                if (_parent != null)
                    _parent.UpdateTransform();
                Matrix2D.Invert(ref _worldTransform, out _worldToLocalTransform);

                _worldToLocalDirty = false;
            }

            return _worldToLocalTransform;
        }
    }
    #endregion

    #region Setters
    public void SetParent(Transform parent)
    {
        if (_parent == parent)
            return;

        if (_parent != null)
            _parent._children.Remove(this);

        if (parent != null)
            parent._children.Add(this);

        _parent = parent;
        SetDirty(Dirt.Position & Dirt.Rotation & Dirt.Scale);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPosition(Vector2 position)
    {
        if (position == _position)
            return;

        _position = position;
        if (_parent != null)
            LocalPosition = WorldToLocalTransform.TransformPoint(position);
        else
            LocalPosition = position;

        _worldPositionDirty = false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRotation(float radians)
    {
        _rotation = radians;
        if (_parent != null)
            LocalRotation = _parent.Rotation + radians;
        else
            LocalRotation = radians;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRotationDegrees(float degrees)
    {
        SetRotation(Lib.Math.DegToRad(degrees));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetScale(Vector2 scale)
    {
        _scale = scale;
        if (_parent != null)
            LocalScale = scale / _parent._scale;
        else
            LocalScale = scale;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetLocalPosition(Vector2 localPosition)
    {
        if (localPosition == _localPosition)
            return;

        _localPosition = localPosition;
        _localDirty = _positionDirty = _worldPositionDirty = true;
        SetDirty(Dirt.Position);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetLocalRotation(float radians)
    {
        _localRotation = radians % MathHelper.TwoPi;
        _localDirty = _rotationDirty = true;
        SetDirty(Dirt.Rotation);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetLocalScale(Vector2 scale)
    {
        _localScale = scale;
        _localDirty = _scaleDirty = true;
        SetDirty(Dirt.Scale);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetDirty(Dirt dirt)
    {
        if ((_dirtyType & dirt) == 0) //not already dirty
        {
            _dirtyType |= dirt;
            for (var i = 0; i < _children.Count; i++)
                _children[i].SetDirty(dirt);
        }
    }

    #endregion

    #region Updaters
    public void UpdateTransform()
    {
        if (_dirtyType != Dirt.Clean)
        {
            if (_parent != null)
                _parent.UpdateTransform();

            if (_localDirty)
            {
                if (_positionDirty)
                {
                    Matrix2D.CreateTranslation(_localPosition, out _positionMatrix);
                    _positionDirty = false;
                }
                if (_rotationDirty)
                {
                    Matrix2D.CreateRotation(_localRotation, out _rotationMatrix);
                    _rotationDirty = false;
                }
                if (_scaleDirty)
                {
                    Matrix2D.CreateScale(_localScale, out _scaleMatrix);
                    _scaleDirty = false;
                }

                Matrix2D.Multiply(ref _scaleMatrix, ref _rotationMatrix, out _localTransform);
                Matrix2D.Multiply(ref _localTransform, ref _positionMatrix, out _localTransform);

                if (_parent == null)
                {
                    _worldTransform = _localTransform;
                    _rotation = _localRotation;
                    _scale = _localScale;
                }
                _localDirty = false;
            }
            if (_parent != null)
            {
                Matrix2D.Multiply(ref _localTransform, ref _parent._worldTransform, out _worldTransform);
                _rotation = _localRotation + _parent._rotation;
                _scale = _parent._scale * _localScale;
            }

            _worldToLocalDirty = true;
            _worldPositionDirty = true;
            _dirtyType = Dirt.Clean;
        }
    }
    #endregion

    public Vector2 LocalToWorldPosition(Vector2 localPosition)
    {
        return LocalToWorldTransform.TransformPoint(localPosition);
    }

    public Vector2 LocalToWorldDirection(Vector2 localDirection)
    {
        return Lib.Math.RotateRadians(localDirection, Rotation);
    }

    public Vector2 WorldToLocalPosition(Vector2 worldPosition)
    {
        return WorldToLocalTransform.TransformPoint(worldPosition);
    }

    public Vector2 WorldToLocalDirection(Vector2 worldDirection)
    {
        return Lib.Math.RotateRadians(worldDirection, -Rotation);
    }
}
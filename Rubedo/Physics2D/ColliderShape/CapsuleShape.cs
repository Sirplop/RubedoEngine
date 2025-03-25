using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Object;
using System;

namespace Rubedo.Physics2D.ColliderShape;

/// <summary>
/// I am CapsuleShape, and this is my summary.
/// </summary>
public class CapsuleShape : IColliderShape
{
    public ShapeType ShapeType => ShapeType.Capsule;
    public Transform Transform
    {
        get
        {
            return _transform;
        }
        set
        {
            _transform = value;
            TransformUpdateRequired = true;
        }
    }
    private Transform _transform;
    public bool TransformUpdateRequired { get => _transformUpdateRequired; set => _transformUpdateRequired = value; }
    private bool _transformUpdateRequired = true;

    public AABB RegisteredBounds { get; set; }
    public ref AABB Bounds
    {
        get
        {
            if (_boundsUpdateRequired)
                RecalculateAABB();
            return ref _bounds;
        }
    }
    protected AABB _bounds;
    public bool BoundsUpdateRequired { get => _boundsUpdateRequired; set => _boundsUpdateRequired = value; }

    private bool _boundsUpdateRequired = true;

    public float Radius { get; protected set; }
    public Vector2 Start {
        get
        {
            if (TransformUpdateRequired)
                TransformVertices();
            return _start;
        }
    }
    public Vector2 End
    {
        get
        {
            if (TransformUpdateRequired)
                TransformVertices();
            return _end;
        }
    }

    private Vector2 _start;
    private Vector2 _end;
    public float Length { get; }
    protected float originalRadius;

    public CapsuleShape(Transform transform, float length, float radius)
    {
        Transform = transform;
        Radius = radius;
        originalRadius = radius;
        _start = new Vector2(0, -length * 0.5f);
        _end = new Vector2(0, length * 0.5f);
        Length = length;
    }

    public void RecalculateAABB()
    {
        if (TransformUpdateRequired)
            TransformVertices();
        float r = MathF.Max(Start.X, End.X) + Radius;
        float l = MathF.Min(Start.X, End.X) - Radius;
        float t = MathF.Max(Start.Y, End.Y) + Radius;
        float b = MathF.Min(Start.Y, End.Y) - Radius;

        _bounds = new AABB { Max = new Vector2(r, t), Min = new Vector2(l, b) };
        BoundsUpdateRequired = false;
    }
    public void TransformVertices()
    {
        if (!TransformUpdateRequired)
            return;
        Matrix2D matrix = Matrix2D.CreateTR(Transform.Position.X, Transform.Position.Y, Transform.Rotation);
        _start = matrix.Transform(new Vector2(0, -Length * 0.5f * (Transform.Scale.Y * 0.5f + 0.5f)));
        _end = matrix.Transform(new Vector2(0, Length * 0.5f * (Transform.Scale.Y * 0.5f + 0.5f)));
        Radius = originalRadius * (Transform.Scale.X * 0.5f + 0.5f);
        _transformUpdateRequired = false;
    }
    public float GetArea()
    {
        //a capsule is 2 halves of the same circle on either end of a rectangle of a given length with width 2R
        //so we just sum the area of the circle and area of the rectangle.
        return (MathHelper.Pi * originalRadius * originalRadius * Transform.Scale.X) + (Length * originalRadius * 2 * Transform.Scale.Y);
    }
    public float GetMomentOfInertia(float mass)
    {
        //might be wrong, idk, it's the right principle
        return (0.5f * mass * Radius * Radius) + (mass * Length / 3f);
    }
}
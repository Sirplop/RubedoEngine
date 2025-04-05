using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Object;
using System;

namespace Rubedo.Physics2D.Collision.Shapes;

/// <summary>
/// I am Capsule, and this is my summary.
/// </summary>
public class Capsule : IShape
{
    public ShapeType ShapeType => ShapeType.Capsule;
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

    internal Transform transform;
    internal float length;
    internal float radius;
    internal Vector2 start;
    internal Vector2 end;

    public Capsule(Transform transform, float length, float radius)
    {
        this.transform = transform;
        this.radius = radius;
        this.length = length;
        start = new Vector2(0, -length * 0.5f);
        end = new Vector2(0, length * 0.5f);
        _bounds = new AABB();
    }

    public float GetArea()
    {
        //a capsule is 2 halves of the same circle on either end of a rectangle of a given length with width 2R
        //so we just sum the area of the circle and area of the rectangle.
        return MathHelper.Pi * radius * radius + length * radius * 2;
    }

    public float GetMomentOfInertia(float mass)
    {
        //might be wrong, idk, it's the right principle
        return 0.5f * mass * radius * radius + mass * length / 3f;
    }

    public void RecalculateAABB()
    {
        GetTransformedPoints(out Vector2 s, out Vector2 e, out float rad);

        float r = MathF.Max(s.X, e.X) + rad;
        float l = MathF.Min(s.X, e.X) - rad;
        float t = MathF.Max(s.Y, e.Y) + rad;
        float b = MathF.Min(s.Y, e.Y) - rad;

        _bounds.Set(new Vector2(l, b), new Vector2(r, t));
        BoundsUpdateRequired = false;
    }

    public void GetTransformedPoints(out Vector2 startR, out Vector2 endR, out float radiusR)
    {
        Vector2 pos = transform.WorldPosition;
        Matrix2D matrix = Matrix2D.CreateTR(pos.X, pos.Y, transform.WorldRotationDegrees);
        Vector2 scale = transform.WorldScale;
        startR = matrix.Transform(new Vector2(0, -length * 0.5f * (scale.Y * 0.5f + 0.5f)));
        endR = matrix.Transform(new Vector2(0, length * 0.5f * (scale.Y * 0.5f + 0.5f)));
        radiusR = radius * (scale.X * 0.5f + 0.5f);
    }
}
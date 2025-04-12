using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Dynamics.Shapes;
using System;

namespace PhysicsEngine2D;

/// <summary>
/// I am Capsule, and this is my summary.
/// </summary>
public class Capsule : Shape
{
    internal float length;
    internal float radius;
    internal Vector2 start;
    internal Vector2 end;

    public Capsule(Transform refTransform, float length, float radius) : base(refTransform)
    {
        type = ShapeType.Capsule;
        this.radius = radius;
        this.length = length;
        start = new Vector2(0, -length * 0.5f);
        end = new Vector2(0, length * 0.5f);
    }

    public override float GetArea()
    {
        //a capsule is 2 halves of the same circle on either end of a rectangle of a given length with width 2R
        //so we just sum the area of the circle and area of the rectangle.
        return MathHelper.Pi * radius * radius + length * radius * 2;
    }

    public override AABB GetBoundingBox()
    {
        GetTransformedPoints(out Vector2 s, out Vector2 e, out float rad);

        float r = MathF.Max(s.X, e.X) + rad;
        float b = MathF.Min(s.Y, e.Y) - rad;
        float l = MathF.Min(s.X, e.X) - rad;
        float t = MathF.Max(s.Y, e.Y) + rad;

        return new AABB(l, b, r, t);
        //_bounds.Set(new Vector2(l, b), new Vector2(r, t));
    }

    public override float GetMomentOfInertia(float mass)
    {
        //might be wrong, idk, it's the right principle
        return 0.5f * mass * radius * radius + mass * length / 3f;
    }

    public override bool Raycast(Ray2 ray, float distance, out RaycastResult result)
    {
        throw new System.NotImplementedException();
    }

    public void GetTransformedPoints(out Vector2 startR, out Vector2 endR, out float radiusR)
    {
        Vector2 pos = transform.Position;
        Matrix2D matrix = Matrix2D.CreateTR(pos.X, pos.Y, transform.Rotation);
        Vector2 scale = transform.Scale;
        startR = matrix.TransformPoint(new Vector2(0, -length * 0.5f * (scale.Y * 0.5f + 0.5f)));
        endR = matrix.TransformPoint(new Vector2(0, length * 0.5f * (scale.Y * 0.5f + 0.5f)));
        radiusR = radius * (scale.X * 0.5f + 0.5f);
    }
}
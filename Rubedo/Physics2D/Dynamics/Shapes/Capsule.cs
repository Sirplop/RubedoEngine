using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Object;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Physics2D.Dynamics.Shapes;

/// <summary>
/// I am Capsule, and this is my summary.
/// </summary>
public class Capsule : Shape
{
    internal float length;
    internal float radius;
    internal Vector2 start;
    internal Vector2 end;

    public Vector2 transStart;
    public Vector2 transEnd;
    public float transRadius;

    public Capsule(float length, float radius)
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
        TransformPoints();

        float r = MathF.Max(transStart.X, transEnd.X) + transRadius;
        float b = MathF.Min(transStart.Y, transEnd.Y) - transRadius;
        float l = MathF.Min(transStart.X, transEnd.X) - transRadius;
        float t = MathF.Max(transStart.Y, transEnd.Y) + transRadius;

        return new AABB(l, b, r, t);
        //_bounds.Set(new Vector2(l, b), new Vector2(r, t));
    }

    public override float GetMomentOfInertia(float mass)
    {
        //might be wrong, idk, it's the right principle
        return 0.5f * mass * radius * radius + mass * length / 3f;
    }

    public override Shape Clone()
    {
        return new Capsule(length, radius);
    }

    public override bool Raycast(Math.Ray2D ray, float distance, out RaycastResult result)
    {
        throw new NotImplementedException();
    }

    public void TransformPoints()
    {
        if (!transformDirty)
            return;

        Vector2 pos = _transform.Position;
        float radians = _transform.Rotation;
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);
        Vector2 scale = _transform.Scale;

        Vector2 s = new Vector2(0, -length * 0.5f * (scale.Y * 0.5f + 0.5f));
        Vector2 e = new Vector2(0, -s.Y);
        TransformPoint(ref s, ref pos, ref sin, ref cos, out transStart);
        TransformPoint(ref e, ref pos, ref sin, ref cos, out transEnd);
        transRadius = radius * (scale.X * 0.5f + 0.5f);

        transformDirty = false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TransformPoint(ref Vector2 value, ref Vector2 pos, ref float sin, ref float cos, out Vector2 result)
    {
        result.X = cos * value.X - sin * value.Y + pos.X;
        result.Y = sin * value.X + cos * value.Y + pos.Y;
    }
}
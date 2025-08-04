using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Object;
using System;

namespace Rubedo.Physics2D.Dynamics.Shapes;

public class Circle : Shape
{
    public float radius;

    public Circle(float radius)
    {
        this.radius = radius;
        type = ShapeType.Circle;
    }


    public override AABB GetBoundingBox()
    {
        Vector2 min = Transform.Position - Vector2.One * radius * Lib.MathV.Max(Transform.Scale);
        Vector2 max = Transform.Position + Vector2.One * radius * Lib.MathV.Max(Transform.Scale);
        return new AABB(min, max);
    }

    public override float GetArea()
    {
        return MathHelper.Pi * radius * radius;
    }
    public override float GetMomentOfInertia(float mass)
    {
        return 0.5f * mass * radius * radius;
    }

    public override Shape Clone()
    {
        return new Circle(radius);
    }

    public override bool Raycast(Math.Ray2D ray, float distance, out RaycastResult result)
    {
        result = new RaycastResult();

        Vector2 delta = ray.origin - Transform.Position;

        // Since  length of ray direction is always 1, therefore a = 1
        float b = 2 * Vector2.Dot(ray.direction, delta);
        float c = delta.LengthSquared() - radius * radius;

        float d = b * b - 4 * c;

        if (d < 0)
        {
            return false;
        }

        float t;

        if (d < Lib.Math.EPSILON)
        {
            t = -b / 2;
        }
        else
        {
            d = (float)System.Math.Sqrt(d);
            t = (-b - d) / 2;
        }

        result.point = ray.origin + ray.direction * t;
        result.distance = t;
        result.normal = Vector2.Normalize(result.point - Transform.Position);

        return t <= distance;
    }
}

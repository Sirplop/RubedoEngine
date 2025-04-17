using Microsoft.Xna.Framework;
using Rubedo.Object;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Dynamics.Shapes;
using Rubedo.Physics2D.Math;
using System;

namespace PhysicsEngine2D;

public class Circle : Shape
{
    public float radius;

    public Circle(Transform refTransform, float radius) : base(refTransform)
    {
        this.radius = radius;
        type = ShapeType.Circle;
    }


    public override AABB GetBoundingBox()
    {
        Vector2 min = transform.Position - Vector2.One * radius * Rubedo.Lib.Math.Max(transform.Scale);
        Vector2 max = transform.Position + Vector2.One * radius * Rubedo.Lib.Math.Max(transform.Scale);
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

    public override bool Raycast(Rubedo.Physics2D.Math.Ray2D ray, float distance, out RaycastResult result)
    {
        result = new RaycastResult();

        Vector2 delta = ray.origin - transform.Position;

        // Since  length of ray direction is always 1, therefore a = 1
        float b = 2 * Vector2.Dot(ray.direction, delta);
        float c = delta.LengthSquared() - radius * radius;

        float d = b * b - 4 * c;

        if (d < 0)
        {
            return false;
        }

        float t;

        if (d < Rubedo.Lib.Math.EPSILON)
        {
            t = -b / 2;
        }
        else
        {
            d = (float)Math.Sqrt(d);
            t = (-b - d) / 2;
        }

        result.point = ray.origin + ray.direction * t;
        result.distance = t;
        result.normal = Vector2.Normalize(result.point - transform.Position);

        return t <= distance;
    }
}

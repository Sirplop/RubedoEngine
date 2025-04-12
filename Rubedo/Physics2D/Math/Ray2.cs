using Microsoft.Xna.Framework;
using System;

namespace PhysicsEngine2D;

public struct Ray2
{
    public const float Tmax = float.MaxValue;

    public Vector2 origin;
    public Vector2 direction;

    public Ray2(Vector2 orig, Vector2 dir)
    {
        origin = orig;
        direction = Vector2.Normalize(dir);
    }

    public bool IntersectSegment(Vector2 a, Vector2 b, out float t)
    {
        return IntersectSegment(a, b, Tmax, out t);
    }

    public bool IntersectSegment(Vector2 a, Vector2 b, float distance, out float t)
    {
        Vector2 v1 = origin - a;
        Vector2 v2 = b - a;
        Vector2 perpD = Rubedo.Lib.Math.Left(direction);

        float denom = Vector2.Dot(v2, perpD);

        if (Math.Abs(denom) < Rubedo.Lib.Math.EPSILON)
        {
            t = Tmax;
            return false;
        }

        t = Rubedo.Lib.Math.Cross(v2, v1) / denom;
        float s = Vector2.Dot(v1, perpD) / denom;

        return t >= 0.0f && s >= 0.0f && s <= 1.0f;
    }
}

using Microsoft.Xna.Framework;

namespace Rubedo.Physics2D.Math;

public struct Ray2D
{
    public const float TMAX = float.MaxValue;

    public Vector2 origin;
    public Vector2 direction;

    public Ray2D(Vector2 orig, Vector2 dir)
    {
        origin = orig;
        direction = Vector2.Normalize(dir);
    }

    public bool IntersectSegment(Vector2 a, Vector2 b, out float t)
    {
        return IntersectSegment(a, b, TMAX, out t);
    }

    public bool IntersectSegment(Vector2 a, Vector2 b, float distance, out float t)
    {
        Vector2 v1 = origin - a;
        Vector2 v2 = b - a;
        Lib.MathV.Left(ref direction, out Vector2 perpD);

        float denom = Vector2.Dot(v2, perpD);

        if (System.Math.Abs(denom) < Lib.Math.EPSILON)
        {
            t = TMAX;
            return false;
        }

        t = Lib.MathV.Cross(v2, v1) / denom;
        float s = Vector2.Dot(v1, perpD) / denom;

        return t >= 0.0f && s >= 0.0f && s <= 1.0f;
    }
}

using Microsoft.Xna.Framework;
using Rubedo.Lib;
using System;
using Math = System.Math;

namespace PhysicsEngine2D;

/// <summary>
/// Axis Aligned bounding box struct that represents the position of an object within a coordinate system.
/// </summary>
public struct AABB : IEquatable<AABB>
{
    public Vector2 min;
    public Vector2 max;

    public float Area => Math.Abs((max.Y - min.Y) * (max.X - min.X));

    public void Set(in Vector2 min, in Vector2 max)
    {
        this.min = min; this.max = max;
    }
    public void Set(in Vector2Int min, in Vector2Int max)
    {
        this.min.X = min.X; this.min.Y = min.Y;
        this.max.X = max.X; this.max.Y = max.Y;
    }

    public AABB(Vector2 min, Vector2 max)
    {
        this.min = min;
        this.max = max;
    }
    public AABB(float left, float bottom, float right, float top)
    {
        this.min = new Vector2(left, bottom);
        this.max = new Vector2(right, top);
    }

    public readonly bool Contains(ref AABB bounds)
    {
        return Vector2.Min(min, bounds.min) == min && Vector2.Max(max, bounds.max) == max;
    }
    public bool Contains(ref Vector2 point)
    {
        return min.X <= point.X &&
               max.X >= point.X &&
               min.Y <= point.Y &&
               max.Y >= point.Y;
    }
    public bool Contains(ref Vector2Int point)
    {
        return min.X <= point.X &&
               max.X >= point.X &&
               min.Y <= point.Y &&
               max.Y >= point.Y;
    }
    public bool Contains(float x, float y)
    {
        return min.X <= x &&
               max.X >= x &&
               min.Y <= y &&
               max.Y >= y;
    }

    public readonly AABB Fatten(float increase)
    {
        return new AABB(min - Vector2.One * increase, max + Vector2.One * increase);
    }

    public readonly bool Overlaps(AABB other)
    {
        return Overlaps(this, other);
    }

    public readonly AABB Union(AABB other)
    {
        return Union(this, other);
    }

    public static AABB Union(AABB a, AABB b)
    {
        return new AABB(Vector2.Min(a.min, b.min), Vector2.Max(a.max, b.max));
    }

    public static void Union(ref AABB bounds1, ref AABB bounds2, out AABB bounds3)
    {
        bounds3 = new AABB()
        {
            min = new Vector2(MathF.Min(bounds1.min.X, bounds2.min.X), MathF.Min(bounds1.min.Y, bounds2.min.Y)),
            max = new Vector2(MathF.Max(bounds1.max.X, bounds2.max.X), MathF.Max(bounds1.max.Y, bounds2.max.Y))
        };
    }
    public static void Union(ref AABB bounds, ref Vector2 point, out AABB bounds2)
    {
        bounds2 = new AABB()
        {
            min = new Vector2(MathF.Min(bounds.min.X, point.X), MathF.Min(bounds.min.Y, point.Y)),
            max = new Vector2(MathF.Max(bounds.max.X, point.X), MathF.Max(bounds.max.Y, point.Y))
        };
    }
    public static void Union(ref AABB bounds, ref Vector2Int point, out AABB bounds2)
    {
        bounds2 = new AABB()
        {
            min = new Vector2(MathF.Min(bounds.min.X, point.X), MathF.Min(bounds.min.Y, point.Y)),
            max = new Vector2(MathF.Max(bounds.max.X, point.X), MathF.Max(bounds.max.Y, point.Y))
        };
    }

    public static bool Overlaps(AABB a, AABB b)
    {
        if (a.max.X < b.min.X || a.min.X > b.max.X) return false;
        if (a.max.Y < b.min.Y || a.min.Y > b.max.Y) return false;

        return true;
    }

    public readonly bool Raycast(Ray2 ray, float distance = Ray2.Tmax)
    {
        float tminX = (min.X - ray.origin.X) / ray.direction.X;
        float tmaxX = (max.X - ray.origin.X) / ray.direction.X;

        float tminY = (min.Y - ray.origin.Y) / ray.direction.Y;
        float tmaxY = (max.Y - ray.origin.Y) / ray.direction.Y;

        float tmin = Math.Max(Math.Min(tminX, tmaxX), Math.Min(tminY, tmaxY));
        float tmax = Math.Min(Math.Max(tminX, tmaxX), Math.Max(tminY, tmaxY));

        if (tmax < 0)
            return false;

        if (tmax < tmin)
            return false;

        return true;
    }

    public bool Equals(AABB other)
    {
        return min.X == other.min.X && max.X == other.max.X &&
            min.Y == other.min.Y && max.Y == other.max.Y;
    }
    public static bool operator ==(AABB left, AABB right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(AABB left, AABB right)
    {
        return !left.Equals(right);
    }
}

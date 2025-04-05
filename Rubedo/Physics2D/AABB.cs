using Microsoft.Xna.Framework;
using Rubedo.Lib;
using System;
using System.Drawing;

namespace Rubedo.Physics2D;

/// <summary>
/// Axis Aligned bounding box struct that represents the position of an object within a coordinate system.
/// </summary>
public struct AABB
{
    public Vector2 Min;
    public Vector2 Max;
    public readonly float Area => MathF.Abs((Max.X - Min.X) * (Max.Y - Min.Y));

    public void Set(in Vector2 min, in Vector2 max)
    {
        this.Min = min; this.Max = max;
    }

    public bool Contains(ref Vector2 point)
    {
        return Min.X <= point.X &&
               Max.X >= point.X &&
               Min.Y <= point.Y &&
               Max.Y >= point.Y;
    }
    public bool Contains(ref Vector2Int point)
    {
        return Min.X <= point.X &&
               Max.X >= point.X &&
               Min.Y <= point.Y &&
               Max.Y >= point.Y;
    }
    public bool Intersects(ref AABB other)
    {
        return Max.X > other.Min.X &&
               Min.X < other.Max.X &&
               Min.Y < other.Max.Y &&
               Max.Y > other.Min.Y;
    }

    public static void Union(ref AABB bounds1, ref AABB bounds2, out AABB bounds3)
    {
        bounds3 = new AABB()
        {
            Min = new Vector2(MathF.Min(bounds1.Min.X, bounds2.Min.X), MathF.Min(bounds1.Min.Y, bounds2.Min.Y)),
            Max = new Vector2(MathF.Max(bounds1.Max.X, bounds2.Max.X), MathF.Max(bounds1.Max.Y, bounds2.Max.Y))
        };
    }
    public static void Union(ref AABB bounds, ref Vector2 point, out AABB bounds2)
    {
        bounds2 = new AABB()
        {
            Min = new Vector2(MathF.Min(bounds.Min.X, point.X), MathF.Min(bounds.Min.Y, point.Y)),
            Max = new Vector2(MathF.Max(bounds.Max.X, point.X), MathF.Max(bounds.Max.Y, point.Y))
        };
    }
    public static void Union(ref AABB bounds, ref Vector2Int point, out AABB bounds2)
    {
        bounds2 = new AABB()
        {
            Min = new Vector2(MathF.Min(bounds.Min.X, point.X), MathF.Min(bounds.Min.Y, point.Y)),
            Max = new Vector2(MathF.Max(bounds.Max.X, point.X), MathF.Max(bounds.Max.Y, point.Y))
        };
    }
}
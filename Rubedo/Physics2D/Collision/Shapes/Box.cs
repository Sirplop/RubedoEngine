using Microsoft.Xna.Framework;
using Rubedo.Object;
using System.Collections.Generic;

namespace Rubedo.Physics2D.Collision.Shapes;

/// <summary>
/// I am Box, and this is my summary.
/// </summary>
public class Box : Polygon
{
    public readonly float width;
    public readonly float height;

    public float Left => vertices[0].X;
    public float Right => vertices[2].X;
    public float Top => vertices[2].Y;
    public float Bottom => vertices[0].Y;

    public Box(Transform transform, float width, float height) : base(transform, BuildBox(width, height))
    {
        this.width = width;
        this.height = height;
    }

    private static List<Vector2> BuildBox(float width, float height)
    {
        width *= 0.5f;
        height *= 0.5f;
        List<Vector2> box = new List<Vector2>
        {
            new Vector2(-width, -height),
            new Vector2(width, -height),
            new Vector2(width, height),
            new Vector2(-width, height)
        };
        return box;
    }

    public override float GetArea()
    {
        return width * height;
    }
    public override float GetMomentOfInertia(float mass)
    {
        return mass / 12f * (width * width + height * height);
    }
}
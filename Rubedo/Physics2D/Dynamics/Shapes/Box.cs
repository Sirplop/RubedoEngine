using Microsoft.Xna.Framework;
using Rubedo.Object;
using System;
using System.Collections.Generic;

namespace Rubedo.Physics2D.Dynamics.Shapes;

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

    protected Box() : base() { }
    public Box(float width, float height) : base()
    {
        type = ShapeType.Box;
        this.width = width;
        this.height = height;

        SetBox(width * 0.5f, height * 0.5f);
        transformDirty = true;
        normalsDirty = true;
    }

    public override float GetArea()
    {
        return width * height;
    }
    public override float GetMomentOfInertia(float mass)
    {
        return mass / 12f * (width * width + height * height);
    }
    public override Shape Clone()
    {
        Box box = new Box();
        box.vertices = new Vector2[VertexCount];
        box.normals = new Vector2[VertexCount];
        Array.Copy(vertices, box.vertices, VertexCount);
        Array.Copy(normals, box.normals, VertexCount);
        box.type = ShapeType.Box;
        box.transformDirty = true;
        box.normalsDirty = true;
        return box;
    }
}
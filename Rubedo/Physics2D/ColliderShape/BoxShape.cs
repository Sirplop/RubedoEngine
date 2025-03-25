using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Object;
using System;
using System.Collections.Generic;

namespace Rubedo.Physics2D.ColliderShape;

/// <summary>
/// I am BoxShape, and this is my summary.
/// </summary>
public class BoxShape : PolygonShape
{
    public readonly float width;
    public readonly float height;

    public float Left => TransformedVertices[0].X;
    public float Right => TransformedVertices[2].X;
    public float Top => TransformedVertices[2].Y;
    public float Bottom => TransformedVertices[0].Y;

    public BoxShape(Transform transform, float width, float height) : base(transform, BuildBox(width, height))
    {
        this.width = width;
        this.height = height;
        IsBox = true;
    }

    private static List<Vector2> BuildBox(float width, float height)
    {
        width = width * 0.5f;
        height = height * 0.5f;
        List<Vector2> box = new List<Vector2>
        {
            new Vector2(-width, -height),
            new Vector2(width, -height),
            new Vector2(width, height),
            new Vector2(-width, height)
        };
        return box;
    }

    public override void TransformVertices()
    {
        if (!TransformUpdateRequired)
            return;
        Matrix2D matrix = Transform.ToMatrix();
        _transformedVertices[0] = matrix.Transform(LocalVertices[0]);
        _transformedVertices[1] = matrix.Transform(LocalVertices[1]);
        _transformedVertices[2] = matrix.Transform(LocalVertices[2]);
        _transformedVertices[3] = matrix.Transform(LocalVertices[3]);
        TransformUpdateRequired = false;
    }

    public override float GetArea()
    {
        return width * height * Transform.Scale.X * Transform.Scale.Y;
    }
    public override float GetMomentOfInertia(float mass)
    {
        return (mass / 12f) * (width * width + height * height);
    }
}
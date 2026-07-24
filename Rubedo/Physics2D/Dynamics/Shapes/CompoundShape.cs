using Loyc.Collections;
using Microsoft.Xna.Framework;
using Rubedo.Graphics;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D.Math;
using System;
using System.Collections.Generic;

namespace Rubedo.Physics2D.Dynamics.Shapes;

/// <summary>
/// Represents a child shape within a compound body, storing its local transform relative to the compound's center of mass.
/// </summary>
public readonly struct ChildShape
{
    public Shape Shape { get; }
    public Vector2 LocalOffset { get; }
    public float LocalAngle { get; }
    public float Mass { get; }
    public float Inertia { get; }

    public ChildShape(Shape shape, Vector2 localOffset, float localAngle, float mass)
    {
        Shape = shape;
        LocalOffset = localOffset;
        LocalAngle = localAngle;
        Mass = mass;
        Inertia = shape.GetMomentOfInertia(mass);
    }
}

public class CompoundShape : Shape
{
    public List<Vector2> LocalVertices { get; set; } = new List<Vector2>();

    public List<ChildShape> Children { get; } = new List<ChildShape>();

    private float _totalInertia;

    public CompoundShape() 
    {
        type = ShapeType.Compound;
        transformDirty = true;
        normalsDirty = true;
    }

    /// <summary>
    /// Adds a child shape to the compound. The localOffset is relative to the compound's center of mass.
    /// </summary>
    public void AddChild(Shape shape, Vector2 localOffset, float localAngle, float mass)
    {
        Transform transform = new Transform(localOffset, localAngle);
        //Translate shapes's local vertices to be local to the compound's center.
        switch (shape.type)
        {
            case ShapeType.Circle:
                break;
            case ShapeType.Capsule:
                Capsule cShape = shape as Capsule;
                //no scaling happens, so length is preserved.
                cShape.start = transform.LocalToWorldPosition(cShape.start);
                cShape.end = transform.LocalToWorldPosition(cShape.end);
                break;
            case ShapeType.Box:
            case ShapeType.Polygon:
                Polygon pShape = (Polygon)shape;
                for (int i = 0; i < pShape.VertexCount; i++)
                {
                    pShape.vertices[i] = transform.LocalToWorldPosition(pShape.vertices[i]);
                }
                break;
            case ShapeType.Compound:
                //TODO: Implement flatterning this into this new compound shape.
                break;

        }
        shape.transformDirty = true;
        shape.normalsDirty = true;

        shape.SetTransform(this._transform); //all children share the transform.
        ChildShape child = new ChildShape(shape, localOffset, localAngle, mass);
        Children.Add(child);

        // Parallel axis theorem: I_total = I_child + m * d^2
        _totalInertia += child.Inertia + mass * Vector2.Dot(localOffset, localOffset);
    }

    public override Shape Clone()
    {
        throw new NotImplementedException();
    }

    public override float GetArea()
    {
        float area = 0;
        for (int i = 0; i < Children.Count; i++)
        {
            ChildShape child = Children[i];
            area += child.Shape.GetArea();
        }
        return area;
    }

    public override AABB GetBoundingBox()
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (ChildShape child in Children)
        {
            if (child.Shape.type == ShapeType.Circle)
            {
                Circle circle = (Circle)child.Shape;
                // Child circles store their local offset; compute their true world position.
                Vector2 worldPos = _transform.LocalToWorldPosition(child.LocalOffset);
                float radius = circle.radius * Lib.MathV.Max(circle.Transform.Scale);
                Vector2 extent = new Vector2(radius, radius);

                Vector2 cmin = worldPos - extent;
                Vector2 cmax = worldPos + extent;

                if (cmin.X < minX) minX = cmin.X;
                if (cmax.X > maxX) maxX = cmax.X;
                if (cmin.Y < minY) minY = cmin.Y;
                if (cmax.Y > maxY) maxY = cmax.Y;
            }
            else
            {
                AABB childAabb = child.Shape.GetBoundingBox();

                if (childAabb.min.X < minX) minX = childAabb.min.X;
                if (childAabb.max.X > maxX) maxX = childAabb.max.X;
                if (childAabb.min.Y < minY) minY = childAabb.min.Y;
                if (childAabb.max.Y > maxY) maxY = childAabb.max.Y;
            }
        }
        return new AABB(minX, minY, maxX, maxY);
    }

    public override float GetMomentOfInertia(float mass)
    {
        return _totalInertia;
    }

    public override bool Raycast(Ray2D ray, float distance, out RaycastResult result)
    {
        throw new NotImplementedException();
    }
}

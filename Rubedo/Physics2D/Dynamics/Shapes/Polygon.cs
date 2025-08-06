using Microsoft.Xna.Framework;
using Rubedo;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rubedo.Physics2D.Dynamics.Shapes;

/// <summary>
/// Note: Vertices in counter-clockwise winding order.
/// </summary>
public class Polygon : Shape
{
    public Vector2[] vertices;
    public Vector2[] normals;
    public Vector2[] transformedVertices;
    public Vector2[] transformedNormals;

    public int VertexCount
    {
        get
        {
            return vertices.Length;
        }
    }

    protected Polygon() { }

    public Polygon(IEnumerable<Vector2> verts)
    {
        SetVertices(verts);
        type = ShapeType.Polygon;
        transformDirty = true;
        normalsDirty = true;
    }

    public Polygon(float halfWidth, float halfHeight)
    {
        SetBox(halfWidth, halfHeight);
        type = ShapeType.Polygon;
        transformDirty = true;
        normalsDirty = true;
    }

    public void SetVertices(IEnumerable<Vector2> verts)
    {
        vertices = verts.ToArray();

        normals = new Vector2[VertexCount]; 
        
        Vector2 centroid = ShapeUtility.ComputeCentroid(vertices);
        for (int i = 0; i < VertexCount; i++)
        {
            vertices[i] -= centroid;
        }

        for (int i = 0; i < VertexCount; i++)
        {
            Vector2 face = vertices[(i + 1) % VertexCount] - vertices[i];
            normals[i] = MathV.Right(face);
            normals[i].Normalize();
        }

        transformedVertices = new Vector2[VertexCount];
        transformedNormals = new Vector2[VertexCount];
    }

    public void SetBox(float halfWidth, float halfHeight)
    {
        Vector2 min = new Vector2(-halfWidth, -halfHeight);
        Vector2 topLeft = new Vector2(-halfWidth, halfHeight);
        SetVertices(new List<Vector2>() { min, -topLeft, -min, topLeft });
    }

    //Generate bounding box for this polygon
    public override AABB GetBoundingBox()
    {
        Vector2 min = _transform.LocalToWorldPosition(vertices[0]);
        Vector2 max = min;

        for (int i = 1; i < VertexCount; i++)
        {
            Vector2 vertex = _transform.LocalToWorldPosition(vertices[i]);
            if (vertex.X < min.X) min.X = vertex.X;
            if (vertex.Y < min.Y) min.Y = vertex.Y;

            if (vertex.X > max.X) max.X = vertex.X;
            if (vertex.Y > max.Y) max.Y = vertex.Y;
        }

        return new AABB(min, max);
    }
    public override float GetArea()
    {        
        // Compute the polygon’s area using the shoelace formula.
        float total = 0f;

        // Sum over edges, possibly negative for CW ordering
        for (int i = 0; i < VertexCount; i++)
        {
            int j = (i + 1) % VertexCount;
            total += vertices[i].X * vertices[j].Y
                - vertices[j].X * vertices[i].Y;
        }

        // Multiply by 0.5 and take absolute value so the area is positive
        return System.Math.Abs(total * 0.5f);
    }
    public override float GetMomentOfInertia(float mass)
    {
        float area = GetArea();
        if (area < Lib.Math.EPSILON)
            return 0f; // Degenerate polygon

        float crossSum = 0f;
        float numer = 0f;

        // Sum over each edge in the polygon.
        for (int i = 0; i < VertexCount; i++)
        {
            int j = (i + 1) % VertexCount;
            Vector2 v0 = vertices[i];
            Vector2 v1 = vertices[j];

            float cross = v0.X * v1.Y - v1.X * v0.Y;
            float termX = v0.X * v0.X + v0.X * v1.X + v1.X * v1.X;
            float termY = v0.Y * v0.Y + v0.Y * v1.Y + v1.Y * v1.Y;

            numer += cross * (termX + termY);
            crossSum += cross;
        }

        crossSum = System.Math.Abs(crossSum);
        if (crossSum < Lib.Math.EPSILON)
            return 0f;  // Nearly degenerate polygon

        // Use the standard formula:
        // I = (mass/(6 * sum(cross))) * numer
        float iPoly = mass * numer / (6f * crossSum);
        return System.Math.Abs(iPoly);
    }

    public override Shape Clone()
    {
        Polygon poly = new Polygon();
        poly.vertices = new Vector2[VertexCount];
        poly.normals = new Vector2[VertexCount];
        Array.Copy(vertices, poly.vertices, VertexCount);
        Array.Copy(normals, poly.normals, VertexCount);
        poly.type = ShapeType.Polygon;
        poly.transformedVertices = new Vector2[VertexCount];
        poly.transformedNormals = new Vector2[VertexCount];
        poly.transformDirty = true;
        poly.normalsDirty = true;
        return poly;
    }

    public override bool Raycast(Ray2D ray, float distance, out RaycastResult result)
    {
        result = new RaycastResult();

        float tmin = Ray2D.TMAX;
        int crossings = 0;

        for (int i = 0; i < VertexCount; i++)
        {
            float t;
            int j = (i + 1) % VertexCount;

            Vector2 a = Transform.LocalToWorldPosition(vertices[i]);
            Vector2 b = Transform.LocalToWorldPosition(vertices[j]);

            if (ray.IntersectSegment(a, b, distance, out t))
            {
                crossings++;
                if (t < tmin && t <= distance)
                {
                    tmin = t;

                    result.point = ray.origin + ray.direction * tmin;
                    //result.normal = transform.LocalToWorldDirection(normals[i]);
                    result.distance = tmin;
                }
            }
        }

        // Point in polygon test, to make sure that origin isn't inside polygon
        return crossings > 0 && crossings % 2 == 0;
    }

    public void TransformVertices()
    {
        if (!transformDirty)
            return;
        for (int i = 0; i < vertices.Length; i++)
        {
            Transform.LocalToWorldPosition(ref vertices[i], out transformedVertices[i]);
        }
        transformDirty = false;
    }
    public void TransformNormals()
    {
        if (!normalsDirty)
            return;

        Vector2 face, vert1, vert2;
        for (int i = 0; i < vertices.Length; i++)
        {
            Transform.LocalToWorldPosition(ref vertices[i], out vert1);
            Transform.LocalToWorldPosition(ref vertices[(i + 1) % VertexCount], out vert2);
            Vector2.Subtract(ref vert2, ref vert1, out face);
            MathV.Right(ref face, out face);
            MathV.Normalize(ref face);
            transformedNormals[i] = face;
        }
        normalsDirty = false;
    }
}

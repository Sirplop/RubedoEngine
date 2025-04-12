using Microsoft.Xna.Framework;
using Rubedo;
using Rubedo.Object;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Dynamics.Shapes;
using Rubedo.Physics2D.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhysicsEngine2D;

/// <summary>
/// Note: Vertices in clockwise winding order.
/// </summary>
public class Polygon : Shape
{
    public Vector2[] vertices;
    public Vector2[] normals;

    public int VertexCount
    {
        get
        {
            return vertices.Length;
        }
    }

    protected Polygon(Transform refTransform) : base(refTransform) { }

    public Polygon(Transform refTransform, IEnumerable<Vector2> verts) : base(refTransform)
    {
        SetVertices(verts);
        type = ShapeType.Polygon;
    }

    public Polygon(Transform refTransform, float halfWidth, float halfHeight) : base(refTransform)
    {
        SetBox(halfWidth, halfHeight);
        type = ShapeType.Polygon;
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
            normals[i] = Rubedo.Lib.Math.Right(face);
            normals[i].Normalize();
        }
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
        if (area < Rubedo.Lib.Math.EPSILON)
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
        if (crossSum < Rubedo.Lib.Math.EPSILON)
            return 0f;  // Nearly degenerate polygon

        // Use the standard formula:
        // I = (mass/(6 * sum(cross))) * numer
        float iPoly = mass * numer / (6f * crossSum);
        return System.Math.Abs(iPoly);
    }

    public override bool Raycast(Ray2 ray, float distance, out RaycastResult result)
    {
        result = new RaycastResult();

        float tmin = Ray2.Tmax;
        int crossings = 0;

        for (int i = 0; i < VertexCount; i++)
        {
            float t;
            int j = (i + 1) % VertexCount;

            Vector2 a = transform.LocalToWorldPosition(vertices[i]);
            Vector2 b = transform.LocalToWorldPosition(vertices[j]);

            if (ray.IntersectSegment(a, b, distance, out t))
            {
                crossings++;
                if (t < tmin && t <= distance)
                {
                    tmin = t;

                    result.point = ray.origin + ray.direction * tmin;
                    result.normal = transform.LocalToWorldDirection(normals[i]);
                    result.distance = tmin;
                }
            }
        }

        // Point in polygon test, to make sure that origin isn't inside polygon
        return crossings > 0 && crossings % 2 == 0;
    }

    //Get furthest vertex on polygon in a direction
    public Vector2 GetSupportPoint(Vector2 dir)
    {
        Vector2 support = Vector2.Zero;
        float maxProjection = float.MinValue;

        foreach(Vector2 vertex in vertices)
        {
            float projection = Vector2.Dot(vertex, dir);

            //If vertex is furthest, projection is greatest
            if(projection > maxProjection)
            {
                maxProjection = projection;
                support = vertex;
            }
        }

        return support;
    }

    public void GetTransformedNormal(int vertex, out Vector2 normal)
    {
        Vector2 face = transform.LocalToWorldPosition(vertices[(vertex + 1) % VertexCount]) - transform.LocalToWorldPosition(vertices[vertex]);
        normal = Rubedo.Lib.Math.Right(face);
        Rubedo.Lib.Math.Normalize(ref normal);
    }

    public bool ContainsPoint(Vector2 point)
    {
        // normalize the point to be in our Polygon coordinate space
        point = transform.WorldToLocalPosition(point);

        bool isInside = false;
        for (int i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
        {
            if (((vertices[i].Y > point.Y) != (vertices[j].Y > point.Y)) &&
                (point.X < (vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) +
                 vertices[i].X))
            {
                isInside = !isInside;
            }
        }

        return isInside;
    }
}

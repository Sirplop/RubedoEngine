using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D.Util;
using System.Collections.Generic;
using System.Linq;

namespace Rubedo.Physics2D.Collision.Shapes;

/// <summary>
/// I am Polygon, and this is my summary.
/// </summary>
public class Polygon : IShape
{
    public ShapeType ShapeType => ShapeType.Polygon;
    public ref AABB Bounds
    {
        get
        {
            if (_boundsUpdateRequired)
                RecalculateAABB();
            return ref _bounds;
        }
    }
    protected AABB _bounds;
    public bool BoundsUpdateRequired { get => _boundsUpdateRequired; set => _boundsUpdateRequired = value; }
    private bool _boundsUpdateRequired = true;

    internal Transform transform;
    internal Vector2[] vertices;
    internal Vector2[] normals;

    public int VertexCount => _vertexCount;
    protected int _vertexCount;

    /// <summary>
    /// Note: Vertices in counter-clockwise winding order.
    /// </summary>
    public Polygon(Transform transform, IEnumerable<Vector2> verts)
    {
        this.transform = transform;
        vertices = verts.ToArray();
        _vertexCount = vertices.Length;
        //move all the points so they're around 0,0.
        Vector2 centroid = ColliderShapeUtility.ComputeCentroid(vertices);
        for (int i = 0; i < _vertexCount; i++)
        {
            vertices[i] -= centroid;
        }

        normals = new Vector2[_vertexCount];

        for (int i = 0; i < VertexCount; i++)
        {
            Vector2 face = vertices[(i + 1) % _vertexCount] - vertices[i];
            normals[i] = Math.Normalize(Math.Right(face, 1));
        }
        _bounds = new AABB();
    }

    public virtual float GetArea()
    {
        // Compute the polygon’s area using the shoelace formula.
        float total = 0f;

        // Sum over edges, possibly negative for CW ordering
        for (int i = 0; i < _vertexCount; i++)
        {
            int j = (i + 1) % _vertexCount;
            total += vertices[i].X * vertices[j].Y
                - vertices[j].X * vertices[i].Y;
        }

        // Multiply by 0.5 and take absolute value so the area is positive
        return System.Math.Abs(total * 0.5f);
    }

    public virtual float GetMomentOfInertia(float mass)
    {
        float area = GetArea();
        if (area < Math.EPSILON)
            return 0f; // Degenerate polygon

        float crossSum = 0f;
        float numer = 0f;

        // Sum over each edge in the polygon.
        for (int i = 0; i < _vertexCount; i++)
        {
            int j = (i + 1) % _vertexCount;
            Vector2 v0 = vertices[i];
            Vector2 v1 = vertices[j];

            float cross = v0.X * v1.Y - v1.X * v0.Y;
            float termX = v0.X * v0.X + v0.X * v1.X + v1.X * v1.X;
            float termY = v0.Y * v0.Y + v0.Y * v1.Y + v1.Y * v1.Y;

            numer += cross * (termX + termY);
            crossSum += cross;
        }

        crossSum = System.Math.Abs(crossSum);
        if (crossSum < Math.EPSILON)
            return 0f;  // Nearly degenerate polygon

        // Use the standard formula:
        // I = (mass/(6 * sum(cross))) * numer
        float iPoly = mass * numer / (6f * crossSum);
        return System.Math.Abs(iPoly);
    }

    public void RecalculateAABB()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        Vector2 point;
        Matrix2D matrix = transform.ToMatrixWorld();
        for (int i = 0; i < _vertexCount; i++)
        {
            point = matrix.Transform(vertices[i]);
            if (point.X < minX) minX = point.X;
            if (point.X > maxX) maxX = point.X;
            if (point.Y < minY) minY = point.Y;
            if (point.Y > maxY) maxY = point.Y;
        }
        _bounds.Set(new Vector2(minX, minY), new Vector2(maxX, maxY));
        BoundsUpdateRequired = false;
    }
    public Vector2 GetSupportPoint(Vector2 dir)
    {
        Vector2 support = Vector2.Zero;
        float maxProjection = float.MinValue;

        foreach (Vector2 vertex in vertices)
        {
            float projection = Vector2.Dot(vertex, dir);

            //If vertex is furthest, projection is greatest
            if (projection > maxProjection)
            {
                maxProjection = projection;
                support = vertex;
            }
        }

        return support;
    }
}
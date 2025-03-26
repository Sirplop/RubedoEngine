using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Object;
using System.Collections.Generic;
using System.Linq;

namespace Rubedo.Physics2D.ColliderShape;

/// <summary>
/// I am PolygonShape, and this is my summary.
/// </summary>
public class PolygonShape : IColliderShape
{
    public ShapeType ShapeType => IsBox ? ShapeType.Box : ShapeType.Polygon;
    public Transform Transform
    {
        get
        {
            return _transform;
        }
        set
        {
            _transform = value;
            TransformUpdateRequired = true;
        }
    }
    private Transform _transform;
    public bool TransformUpdateRequired { get => _transformUpdateRequired; set => _transformUpdateRequired = value; }
    protected bool _transformUpdateRequired = true;

    public AABB RegisteredBounds { get; set; }
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

    /// <summary>
    /// Local-space vertices of the polygon, in clockwise or counterclockwise order.
    /// </summary>
    public Vector2[] LocalVertices { get; protected set; }
    /// <summary>
    /// World-space vertices of the polygon, updated by setting <see cref="TransformUpdateRequired"/>.
    /// </summary>
    public Vector2[] TransformedVertices { 
        get
        {
            if (TransformUpdateRequired)
                TransformVertices();
            return _transformedVertices;
        }
        protected set
        {
            _transformedVertices = value;
        }
    }
    protected Vector2[] _transformedVertices;

    /// <summary>
    /// edge normals are used for SAT collision detection. We cache them to avoid the squareroots.
    /// </summary>
    public Vector2[] EdgeNormals
    {
        get
        {
            if (areEdgeNormalsDirty)
                BuildEdgeNormals();
            return _edgeNormals;
        }
    }

    public bool areEdgeNormalsDirty = true;
    public Vector2[] _edgeNormals;

    public bool IsBox = false;

    public PolygonShape(Transform transform, IEnumerable<Vector2> vertices)
    {
        Transform = transform;
        LocalVertices = vertices.ToArray();
        Vector2 centroid = ColliderShapeUtility.ComputeCentroid(LocalVertices);

        areEdgeNormalsDirty = true; 

        // Shift each vertex so the centroid is at (0,0)
        for (int i = 0; i < LocalVertices.Length; i++)
        {
            LocalVertices[i] -= centroid;
        }
        TransformedVertices = new Vector2[LocalVertices.Length];
        TransformUpdateRequired = true;
    }
    public virtual void RecalculateAABB()
    {
        if (TransformUpdateRequired)
            TransformVertices();
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        Vector2 point;
        for (int i = 0; i < _transformedVertices.Length; i++)
        {
            point = _transformedVertices[i];
            if (point.X < minX) minX = point.X;
            if (point.X > maxX) maxX = point.X;
            if (point.Y < minY) minY = point.Y;
            if (point.Y > maxY) maxY = point.Y;
        }

        _bounds = new AABB
        {
            Min = new Vector2(minX, minY),
            Max = new Vector2(maxX, maxY)
        };
    }

    public virtual void TransformVertices()
    {
        if (!TransformUpdateRequired)
            return;
        Matrix2D matrix = Transform.ToMatrix();
        for (int i = 0; i < LocalVertices.Length; i++)
        {
            _transformedVertices[i] = matrix.Transform(LocalVertices[i]);
        }
        TransformUpdateRequired = false;
    }


    /// <summary>
    /// builds the Polygon edge normals. These are lazily created and updated only by the edgeNormals getter
    /// </summary>
    protected void BuildEdgeNormals()
    {
        var totalEdges = IsBox ? 2 : TransformedVertices.Length;
        if (_edgeNormals == null || _edgeNormals.Length != totalEdges)
            _edgeNormals = new Vector2[totalEdges];

        Vector2 p2;
        for (var i = 0; i < totalEdges; i++)
        {
            var p1 = TransformedVertices[i];
            if (i + 1 >= TransformedVertices.Length)
                p2 = TransformedVertices[0];
            else
                p2 = TransformedVertices[i + 1];

            var perp = Vector2Ext.Perpendicular(ref p1, ref p2);
            Vector2Ext.Normalize(ref perp);
            _edgeNormals[i] = perp;
        }
        areEdgeNormalsDirty = false;
        return;
    }

    /// <summary>
    /// Returns the polygon's area (using the shoelace formula), 
    /// always returning a positive value for both CW and CCW vertices.
    /// </summary>
    public virtual float GetArea()
    {
        float total = 0f;
        int count = LocalVertices.Length;

        // Sum over edges, possibly negative for CW ordering
        for (int i = 0; i < count; i++)
        {
            int j = (i + 1) % count;
            total += (LocalVertices[i].X * LocalVertices[j].Y)
                - (LocalVertices[j].X * LocalVertices[i].Y);
        }

        // Multiply by 0.5 and take absolute value so the area is positive
        return System.Math.Abs(total * 0.5f * Transform.Scale.X * Transform.Scale.Y);
    }
    public virtual float GetMomentOfInertia(float mass)
    {
        // 1) Compute the polygon’s area using the shoelace formula.
        float area = GetArea();
        if (area < Lib.Math.EPSILON)
            return 0f; // Degenerate polygon

        float crossSum = 0f;
        float numer = 0f;

        // 2) Sum over each edge in the polygon.
        for (int i = 0; i < LocalVertices.Length; i++)
        {
            int j = (i + 1) % LocalVertices.Length;
            Vector2 v0 = LocalVertices[i];
            Vector2 v1 = LocalVertices[j];

            float cross = (v0.X * v1.Y - v1.X * v0.Y);
            float termX = (v0.X * v0.X) + (v0.X * v1.X) + (v1.X * v1.X);
            float termY = (v0.Y * v0.Y) + (v0.Y * v1.Y) + (v1.Y * v1.Y);

            numer += cross * (termX + termY);
            crossSum += cross;
        }

        crossSum = System.Math.Abs(crossSum);
        if (crossSum < 1e-8f)
            return 0f;  // Nearly degenerate polygon

        // 3) Use the standard formula:
        // I = (mass/(6 * sum(cross))) * numer
        float iPoly = (mass * numer) / (6f * crossSum);
        return System.Math.Abs(iPoly);
    }

    #region Static Helpers
    /// <summary>
    /// iterates all the edges of the polygon and gets the closest point on any edge to point. Returns via out the squared distance
    /// to the closest point and the normal of the edge it is on. point should be in the space of the Polygon (point - poly.position)
    /// </summary>
    /// <returns>The closest point on polygon to point.</returns>
    /// <param name="point">Point.</param>
    /// <param name="distanceSquared">Distance squared.</param>
    /// <param name="edgeNormal">Edge normal.</param>
    public static Vector2 GetClosestPointOnPolygonToPoint(Vector2[] points, Vector2 point,
                                                          out float distanceSquared, out Vector2 edgeNormal)
    {
        distanceSquared = float.MaxValue;
        edgeNormal = Vector2.Zero;
        var closestPoint = Vector2.Zero;

        float tempDistanceSquared;
        for (var i = 0; i < points.Length; i++)
        {
            var j = i + 1;
            if (j == points.Length)
                j = 0;

            Vector2 closest = Collisions.ClosestPointOnLine(points[i], points[j], point);
            Vector2.DistanceSquared(ref point, ref closest, out tempDistanceSquared);

            if (tempDistanceSquared < distanceSquared)
            {
                distanceSquared = tempDistanceSquared;
                closestPoint = closest;

                // get the normal of the line
                var line = points[j] - points[i];
                edgeNormal.X = -line.Y;
                edgeNormal.Y = line.X;
            }
        }

        Vector2Ext.Normalize(ref edgeNormal);

        return closestPoint;
    }
    #endregion
}
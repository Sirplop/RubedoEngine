using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo.Lib;
using System.Collections.Generic;

namespace Rubedo.Physics2D.Math;

/// <summary>
/// I am ShapeUtility, and this is my summary.
/// </summary>
public class ShapeUtility
{
    /// <summary>
    /// Computes the centroid (center of mass) of a polygon.
    /// </summary>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static Vector2 ComputeCentroid(Vector2[] polygon)
    {
        float accumulatedArea = 0f;
        float centerX = 0f;
        float centerY = 0f;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i, i++)
        {
            float temp = polygon[j].X * polygon[i].Y - polygon[i].X * polygon[j].Y;
            accumulatedArea += temp;
            centerX += (polygon[j].X + polygon[i].X) * temp;
            centerY += (polygon[j].Y + polygon[i].Y) * temp;
        }

        // Much greater than the recommended epsilon of 1e-6, but the system becomes unstable below this
        if (System.Math.Abs(accumulatedArea) < 0.5f)
            return polygon[0];

        // Multiply accumulatedArea by 3 to get the proper divisor (6 * area).
        accumulatedArea *= 3f;
        return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
    }
    public static Vector2 ComputeCentroid(List<Vector2> polygon)
    {
        float accumulatedArea = 0f;
        float centerX = 0f;
        float centerY = 0f;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i, i++)
        {
            float temp = polygon[j].X * polygon[i].Y - polygon[i].X * polygon[j].Y;
            accumulatedArea += temp;
            centerX += (polygon[j].X + polygon[i].X) * temp;
            centerY += (polygon[j].Y + polygon[i].Y) * temp;
        }

        // Much greater than the recommended epsilon of 1e-6, but the system becomes unstable below this
        if (System.Math.Abs(accumulatedArea) < 0.5f)
            return polygon[0];

        // Multiply accumulatedArea by 3 to get the proper divisor (6 * area).
        accumulatedArea *= 3f;
        return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
    }

    /// <summary>
    /// Computes the triangles for a convex polygon with <paramref name="vertexCount"/> vertices.
    /// </summary>
    public static int[] ComputeTriangles(int vertexCount)
    {
        int[] ret = new int[(vertexCount - 2) * 3];
        for (int i = 0; i < vertexCount - 2; i++)
        {
            ret[i * 3] = 0;
            ret[(i * 3) + 1] = i + 1;
            ret[(i * 3) + 2] = i + 2;
        }
        return ret;
    }
    /// <summary>
    /// Gets the closest point on line <paramref name="A"/><paramref name="B"/> to the given <paramref name="point"/>.
    /// </summary>
    public static Vector2 ClosestPointOnLine(in Vector2 A, in Vector2 B, in Vector2 point)
    {
        Vector2 AB = B - A;
        float t = Vector2.Dot(point - A, AB) / Vector2.Dot(AB, AB);
        return A + Lib.Math.Clamp(t, 0, 1) * AB;
    }

    public static Vector2 GetClosestPointOnPolygon(Polygon poly, Vector2 point,
                                                   out float distanceSquared, out Vector2 edgeNormal)
    {
        distanceSquared = float.MaxValue;
        edgeNormal = Vector2.Zero;
        Vector2 closestPoint = Vector2.Zero;

        float tempDistanceSquared;
        Vector2 offPoint = poly.transform.LocalToWorldPosition(poly.vertices[0]);
        Vector2 b;
        Vector2 closest;
        for (int i = 0; i < poly.VertexCount; i++)
        {
            b = poly.transform.LocalToWorldPosition(poly.vertices[(i + 1) % poly.VertexCount]);

            closest = ClosestPointOnLine(offPoint, b, point);
            Vector2.DistanceSquared(ref point, ref closest, out tempDistanceSquared);

            if (tempDistanceSquared < distanceSquared)
            {
                distanceSquared = tempDistanceSquared;
                closestPoint = closest;

                // get the normal of the line
                Vector2 line = b - offPoint;
                edgeNormal.X = -line.Y;
                edgeNormal.Y = line.X;
            }
            offPoint = b;
        }

        Rubedo.Lib.Math.Normalize(ref edgeNormal);

        return closestPoint;
    }
}
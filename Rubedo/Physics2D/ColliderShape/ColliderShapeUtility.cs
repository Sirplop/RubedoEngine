using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubedo.Physics2D.ColliderShape;

/// <summary>
/// I am ColliderShapeUtility, and this is my summary.
/// </summary>
public static class ColliderShapeUtility
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
        if (Math.Abs(accumulatedArea) < 0.5f)
            return polygon[0];

        // Multiply accumulatedArea by 3 to get the proper divisor (6 * area).
        accumulatedArea *= 3f;
        return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
    }
    public static Vector2 FindClosestPointOnPolygon(Vector2 point, Vector2[] vertices)
    {
        int result = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v = vertices[i];
            float distanceSqr = Vector2.DistanceSquared(v, point);

            if (distanceSqr < minDistance)
            {
                minDistance = distanceSqr;
                result = i;
            }
        }

        return vertices[result];
    }
    /// <summary>
    /// gets the closest point that is on the rectangle border to the given point
    /// </summary>
    /// <returns>The closest point on rectangle border to point.</returns>
    /// <param name="point">Point.</param>
    public static Vector2 FindClosestPointOnBox(in BoxShape box, Vector2 point, out Vector2 edgeNormal)
    {
        edgeNormal = Vector2.Zero;

        // for each axis, if the point is outside the box clamp it to the box else leave it alone
        Vector2 res = new Vector2();
        res.X = MathHelper.Clamp(point.X, box.Left, box.Right);
        res.Y = MathHelper.Clamp(point.Y, box.Bottom, box.Top);

        // if point is inside the rectangle we need to push res to the border since it will be inside the rect
        if (Collisions.BoxContainsPoint(box, res))
        {
            var dl = res.X - box.Left;
            var dr = box.Right - res.X;
            var db = res.Y - box.Bottom;
            var dt = box.Top - res.Y;

            var min = MathF.Min(MathF.Min(dl, dr), MathF.Min(dt, db));
            if (min == dt)
            {
                res.Y = box.Top;
                edgeNormal.Y = 1;
            }
            else if (min == db)
            {
                res.Y = box.Bottom;
                edgeNormal.Y = -1;
            }
            else if (min == dl)
            {
                res.X = box.Left;
                edgeNormal.X = -1;
            }
            else
            {
                res.X = box.Right;
                edgeNormal.X = 1;
            }
        }
        else
        {
            if (res.X == box.Left)
                edgeNormal.X = -1;
            if (res.X == box.Right)
                edgeNormal.X = 1;
            if (res.Y == box.Top)
                edgeNormal.Y = 1;
            if (res.Y == box.Bottom)
                edgeNormal.Y = -1;
        }

        return res;
    }


    public static void FindContactPoints(Vector2[] verticesA, Vector2[] verticesB,
            out Vector2 contact1, out Vector2 contact2, out int contactCount)
    {
        contact1 = Vector2.Zero;
        contact2 = Vector2.Zero;
        contactCount = 0;

        float minDistSq = float.MaxValue;

        for (int i = 0; i < verticesA.Length; i++)
        {
            Vector2 p = verticesA[i];

            for (int j = 0; j < verticesB.Length; j++)
            {
                Vector2 va = verticesB[j];
                Vector2 vb = verticesB[(j + 1) % verticesB.Length];

                Vector2 cp = Collisions.ClosestPointOnLine(va, vb, p);
                float distSq = Vector2.DistanceSquared(cp, p);

                if (Lib.Math.NearlyEqual(distSq, minDistSq))
                {
                    if (!Lib.Math.NearlyEqual(cp, contact1) &&
                        !Lib.Math.NearlyEqual(cp, contact2))
                    {
                        contact2 = cp;
                        contactCount = 2;
                    }
                }
                else if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    contactCount = 1;
                    contact1 = cp;
                }
            }
        }

        for (int i = 0; i < verticesB.Length; i++)
        {
            Vector2 p = verticesB[i];

            for (int j = 0; j < verticesA.Length; j++)
            {
                Vector2 va = verticesA[j];
                Vector2 vb = verticesA[(j + 1) % verticesA.Length];

                Vector2 cp = Collisions.ClosestPointOnLine(va, vb, p);
                float distSq = Vector2.DistanceSquared(cp, p);

                if (Lib.Math.NearlyEqual(distSq, minDistSq))
                {
                    if (!Lib.Math.NearlyEqual(cp, contact1) &&
                        !Lib.Math.NearlyEqual(cp, contact2))
                    {
                        contact2 = cp;
                        contactCount = 2;
                    }
                }
                else if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    contactCount = 1;
                    contact1 = cp;
                }
            }
        }
    }
}
using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Physics2D.Collision.Shapes;
using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Rubedo.Physics2D;

/// <summary>
/// Basic Collision tests, only tell you if the colliders touched at all.
/// Much faster than the physics ones.
/// </summary>
public static class Collisions
{
    [Flags]
    public enum PointSectors
    {
        Center = 0,
        Top = 1,
        Bottom = 2,
        TopLeft = 9,
        TopRight = 5,
        Left = 8,
        Right = 4,
        BottomLeft = 10,
        BottomRight = 6
    };

    #region Point
    public static bool BoxContainsPoint(in Box box, in Vector2 point)
    {
        return point.X <= box.Right &&   //is the point LEFT of the RIGHT edge?
            point.X >= box.Left &&       //is the point RIGHT of the LEFT edge?
            point.Y <= box.Top &&        //is the point BELOW the TOP edge?
            point.Y >= box.Bottom;       //is the point ABOVE the BOTTOM edge?
    }
    /// <summary>
    /// essentially what the algorithm is doing is shooting a ray from point out. If it intersects an odd number of polygon sides
    /// we know it is inside the polygon.
    /// </summary>
    public static bool PolygonContainsPoint(in Polygon poly, in Vector2 point)
    {
        var isInside = false;
        Vector2 p = point - poly.transform.WorldPosition;
        for (int i = 0, j = poly.vertices.Length - 1; i < poly.vertices.Length; j = i++)
        {
            if (((poly.vertices[i].Y > p.Y) != (poly.vertices[j].Y > p.Y)) &&
                (p.X < (poly.vertices[j].X - poly.vertices[i].X) * (p.Y - poly.vertices[i].Y) / (poly.vertices[j].Y - poly.vertices[i].Y) +
                  poly.vertices[i].X))
            {
                isInside = !isInside;
            }
        }

        return isInside;
    }
    #endregion

    #region Line
    public static Vector2 ClosestPointOnLine(in Vector2 A, in Vector2 B, in Vector2 point)
    {
        Vector2 AB = B - A;
        float t = Vector2.Dot(point - A, AB) / Vector2.Dot(AB, AB);
        return A + Lib.Math.Clamp(t, 0, 1) * AB;
    }

    public static bool LineLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        Vector2 b = a2 - a1;
        Vector2 d = b2 - b1;
        float bDotDPerp = b.X * d.Y - b.Y * d.X;

        // if b dot d == 0, it means the lines are parallel so have infinite intersection points
        if (bDotDPerp == 0)
            return false;

        Vector2 c = b1 - a1;
        float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
        if (t < 0 || t > 1)
            return false;

        float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
        if (u < 0 || u > 1)
            return false;

        return true;
    }


    public static bool LineLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
    {
        intersection = Vector2.Zero;

        var b = a2 - a1;
        var d = b2 - b1;
        var bDotDPerp = b.X * d.Y - b.Y * d.X;

        // if b dot d == 0, it means the lines are parallel so have infinite intersection points
        if (bDotDPerp == 0)
            return false;

        var c = b1 - a1;
        var t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
        if (t < 0 || t > 1)
            return false;

        var u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
        if (u < 0 || u > 1)
            return false;

        intersection = a1 + t * b;

        return true;
    }
    #endregion
    #region Circle
    public static bool CircleCircle(Vector2 circleCenter1, float circleRadius1, Vector2 circleCenter2, float circleRadius2)
    {
        return Vector2.DistanceSquared(circleCenter1, circleCenter2) < 
            (circleRadius1 + circleRadius2) * (circleRadius1 + circleRadius2);
    }
    public static bool CircleLine(Vector2 circleCenter, float radius, Vector2 lineFrom, Vector2 lineTo)
    {
        return Vector2.DistanceSquared(circleCenter, ClosestPointOnLine(lineFrom, lineTo, circleCenter)) <
               radius * radius;
    }
    public static bool CircleToPoint(Vector2 circleCenter, float radius, Vector2 point)
    {
        return Vector2.DistanceSquared(circleCenter, point) < radius * radius;
    }

    #endregion

#if FALSE
    #region Box
    public static bool BoxCircle(BoxShape box, Vector2 boxPos, Vector2 cPosition, float cRadius)
    {
        return RectToCircle(boxPos.X, boxPos.Y, box.width, box.height, cPosition, cRadius);
    }
    public static bool RectToCircle(float rectX, float rectY, float rectWidth, float rectHeight,
                                        Vector2 circleCenter, float radius)
    {
        //Check if the rectangle contains the circle's center-point
        if (RectToPoint(rectX, rectY, rectWidth, rectHeight, circleCenter))
            return true;

        // Check the circle against the relevant edges
        Vector2 edgeFrom;
        Vector2 edgeTo;
        var sector = GetSector(rectX, rectY, rectWidth, rectHeight, circleCenter);

        if ((sector & PointSectors.Top) != 0)
        {
            edgeFrom = new Vector2(rectX, rectY);
            edgeTo = new Vector2(rectX + rectWidth, rectY);
            if (CircleLine(circleCenter, radius, edgeFrom, edgeTo))
                return true;
        }

        if ((sector & PointSectors.Bottom) != 0)
        {
            edgeFrom = new Vector2(rectX, rectY + rectHeight);
            edgeTo = new Vector2(rectX + rectWidth, rectY + rectHeight);
            if (CircleLine(circleCenter, radius, edgeFrom, edgeTo))
                return true;
        }

        if ((sector & PointSectors.Left) != 0)
        {
            edgeFrom = new Vector2(rectX, rectY);
            edgeTo = new Vector2(rectX, rectY + rectHeight);
            if (CircleLine(circleCenter, radius, edgeFrom, edgeTo))
                return true;
        }

        if ((sector & PointSectors.Right) != 0)
        {
            edgeFrom = new Vector2(rectX + rectWidth, rectY);
            edgeTo = new Vector2(rectX + rectWidth, rectY + rectHeight);
            if (CircleLine(circleCenter, radius, edgeFrom, edgeTo))
                return true;
        }

        return false;
    }
    #endregion
#endif
    #region Capsule

    #endregion
    #region Polygon

    #endregion

    #region Sectors

    /*
     *  Bitflags and helpers for using the Cohen–Sutherland algorithm
     *  http://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
     *  
     *  Sector bitflags:
     *      1001  1000  1010
     *      0001  0000  0010
     *      0101  0100  0110
     */

    public static PointSectors GetSector(float rX, float rY, float rW, float rH, Vector2 point)
    {
        PointSectors sector = PointSectors.Center;

        if (point.X < rX)
            sector |= PointSectors.Left;
        else if (point.X >= rX + rW)
            sector |= PointSectors.Right;

        if (point.Y < rY)
            sector |= PointSectors.Top;
        else if (point.Y >= rY + rH)
            sector |= PointSectors.Bottom;

        return sector;
    }

    #endregion
}
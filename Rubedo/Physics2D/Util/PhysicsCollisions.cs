using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Physics2D.ColliderShape;
using System;
using System.Collections.Generic;

namespace Rubedo.Physics2D.Util;

/// <summary>
/// Collision detection for physics, getting the penetration depth and normal of collision.
/// </summary>
public static class PhysicsCollisions
{
    #region Helper
    public static bool CheckCollide(IColliderShape shape1, IColliderShape shape2, ref CollisionManifold result)
    {
        bool ret;
        switch (shape1.ShapeType)
        {
            case ShapeType.Circle:
                CircleShape circle1 = (CircleShape)shape1;
                switch (shape2.ShapeType)
                {
                    case ShapeType.Circle:
                        CircleShape circle2 = (CircleShape)shape2;
                        return CircleCircle(circle1.Transform.Position, circle1.Radius, circle2.Transform.Position, circle2.Radius, ref result);
                    case ShapeType.Box:
                        BoxShape box2 = (BoxShape)shape2;
                        return CirclePolygon(box2, circle1, ref result);
                    case ShapeType.Capsule:
                        CapsuleShape capsule2 = (CapsuleShape)shape2;
                        return CircleCapsule(circle1, capsule2, ref result);
                    case ShapeType.Polygon:
                        PolygonShape poly2 = (PolygonShape)shape2;
                        return CirclePolygon(poly2, circle1, ref result);
                }
                break;
            case ShapeType.Box:
                BoxShape box1 = (BoxShape)shape1;
                switch (shape2.ShapeType)
                {
                    case ShapeType.Circle:
                        CircleShape circle2 = (CircleShape)shape2;
                        ret = CirclePolygon(box1, circle2, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Box:
                        BoxShape box2 = (BoxShape)shape2;
                        return BoxBox(box1, box2, ref result);
                    case ShapeType.Capsule:
                        CapsuleShape capsule2 = (CapsuleShape)shape2;
                        ret = CapsulePolygon(capsule2, box1, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Polygon:
                        PolygonShape poly2 = (PolygonShape)shape2;
                        return PolygonPolygon(box1, poly2, ref result);
                }
                break;
            case ShapeType.Capsule:
                CapsuleShape capsule1 = (CapsuleShape)shape1;
                switch (shape2.ShapeType)
                {
                    case ShapeType.Circle:
                        CircleShape circle2 = (CircleShape)shape2;
                        ret = CircleCapsule(circle2, capsule1, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Box:
                        BoxShape box2 = (BoxShape)shape2;
                        return CapsulePolygon(capsule1, box2, ref result);
                    case ShapeType.Capsule:
                        CapsuleShape capsule2 = (CapsuleShape)shape2;
                        return CapsuleCapsule(capsule1, capsule2, ref result);
                    case ShapeType.Polygon:
                        PolygonShape poly2 = (PolygonShape)shape2;
                        return CapsulePolygon(capsule1, poly2, ref result);
                }
                break;
            case ShapeType.Polygon:
                PolygonShape poly1 = (PolygonShape)shape1;
                switch (shape2.ShapeType)
                {
                    case ShapeType.Circle:
                        CircleShape circle2 = (CircleShape)shape2;
                        ret = CirclePolygon(poly1, circle2, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Box:
                        BoxShape box2 = (BoxShape)shape2;
                        return PolygonPolygon(poly1, box2, ref result);
                    case ShapeType.Capsule:
                        CapsuleShape capsule2 = (CapsuleShape)shape2;
                        ret = CapsulePolygon(capsule2, poly1, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Polygon:
                        PolygonShape poly2 = (PolygonShape)shape2;
                        return PolygonPolygon(poly1, poly2, ref result);
                }
                break;
        }
        return false;
    }
    #endregion


    #region Circle
    public static bool CircleCircle(Vector2 circleCenter1, float circleRadius1, Vector2 circleCenter2,
                                      float circleRadius2, ref CollisionManifold result)
    {
        float sqrDist = Vector2.DistanceSquared(circleCenter1, circleCenter2);
        if (sqrDist >= (circleRadius1 + circleRadius2) * (circleRadius1 + circleRadius2))
        {
            return false;
        }
        result.normal = Vector2.Normalize(circleCenter2 - circleCenter1);
        result.depth = circleRadius1 + circleRadius2 - MathF.Sqrt(sqrDist);
        result.contactPoint1 = circleCenter2 + result.normal * circleRadius2;

        return true;
    }
    public static bool CircleBox(CircleShape circle, BoxShape box, ref CollisionManifold result)
    {
        // Boxes that have undergone rotations must be calculated as polygons.
        if (box.Transform.Rotation != 0)
            return CirclePolygon(box, circle, ref result);

        result.depth = 0;

        result.contactPoint1 = ColliderShapeUtility.FindClosestPointOnBox(box, circle.Transform.Position, out result.normal);

        // deal with circles whos center is in the box first since its cheaper to see if we are contained
        if (Collisions.BoxContainsPoint(box, circle.Transform.Position))
        {
            result.normal = Lib.Math.Normalize(result.normal);
            //kinda janky, but it works
            Vector2 res = circle.Transform.Position * result.normal - result.contactPoint1 * result.normal;
            if (res.Y == 0)
                result.depth = MathF.Abs(res.X) + circle.Radius;
            else
                result.depth = MathF.Abs(res.Y) + circle.Radius;
            //result.normal = -result.normal;
            return true;
        }

        float sqrDistance = Vector2.DistanceSquared(result.contactPoint1, circle.Transform.Position);

        // see if the point on the box is less than radius from the circle
        if (sqrDistance <= circle.Radius * circle.Radius)
        {
            result.normal = circle.Transform.Position - result.contactPoint1;
            result.depth = result.normal.Length() - circle.Radius;

            result.normal = Vector2Ext.Normalize(result.normal);

            return true;
        }

        return false;
    }
    public static bool CircleCapsule(CircleShape circle, CapsuleShape capsule, ref CollisionManifold result)
    {
        //we pick the "most relevant circle" to do collision with.
        //Basically, we find the closest point to our shape on the capsule line
        //and do a circle collision there.
        Vector2 closestPoint = Collisions.ClosestPointOnLine(capsule.Start, capsule.End, circle.Transform.Position);
        return CircleCircle(circle.Transform.Position, circle.Radius, closestPoint, capsule.Radius, ref result);
    }

    public static bool CirclePolygon(in PolygonShape poly, in CircleShape circle, ref CollisionManifold result)
    {
        return CirclePolygon(poly, circle.Transform.Position, circle.Radius, ref result);
    }
    public static bool CirclePolygon(PolygonShape poly, Vector2 circPos, float radius, ref CollisionManifold result)
    {
        result.depth = float.MaxValue;

        float distanceSquared;
        Vector2 closestPoint = PolygonShape.GetClosestPointOnPolygonToPoint(poly.TransformedVertices, circPos, out distanceSquared,
            out result.normal);
        bool circleCenterInsidePoly = Collisions.PolygonContainsPoint(poly, circPos);
        if (distanceSquared > radius * radius && !circleCenterInsidePoly)
            return false;

        Vector2 axis;
        float axisDepth;
        float minA, maxA, minB, maxB;

        for (int i = 0; i < poly.TransformedVertices.Length; i++)
        {
            Vector2 va = poly.TransformedVertices[i];
            Vector2 vb = poly.TransformedVertices[(i + 1) % poly.TransformedVertices.Length];

            Vector2 edge = vb - va;
            axis = Lib.Math.Normalize(-edge.Y, edge.X);

            ProjectVertices(poly.TransformedVertices, axis, out minA, out maxA);
            ProjectCircle(circPos, radius, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < result.depth)
            {
                result.depth = axisDepth;
                result.normal = axis;
            }
        }

        axis = closestPoint - circPos;
        axis = Lib.Math.Normalize(axis);

        ProjectVertices(poly.TransformedVertices, axis, out minA, out maxA);
        ProjectCircle(circPos, radius, axis, out minB, out maxB);

        if (minA >= maxB || minB >= maxA)
        {
            return false;
        }

        axisDepth = MathF.Min(maxB - minA, maxA - minB);

        if (axisDepth < result.depth)
        {
            result.depth = axisDepth;
            result.normal = axis;
        }

        Vector2 direction = poly.Transform.Position - circPos;

        if (Vector2.Dot(direction, result.normal) < 0f)
        {
            result.normal = -result.normal;
        }

        result.contactPoint1 = circPos + result.normal * radius;

        return true;
    }
    private static void ProjectCircle(Vector2 center, float radius, Vector2 axis, out float min, out float max)
    {
        Vector2 dirAndRadius = axis * radius;

        Vector2 p1 = center + dirAndRadius;
        Vector2 p2 = center - dirAndRadius;

        min = Vector2.Dot(p1, axis);
        max = Vector2.Dot(p2, axis);

        if (min > max)
        {
            float t = min;
            min = max;
            max = t;
        }
    }
    #endregion
    #region Box
    public static bool BoxBox(in BoxShape box1, in BoxShape box2, ref CollisionManifold result)
    {
        //TODO: Find a box-box collider. It should be SUPER fast.
        return PolygonPolygon(box1, box2, ref result);
    }
    #endregion
    #region Capsule
    public static bool CapsuleCapsule(in CapsuleShape capsule1, in CapsuleShape capsule2, ref CollisionManifold result)
    {
        //vectors between line endpoints
        Vector2 v0 = capsule2.Start - capsule1.Start;
        Vector2 v1 = capsule2.End - capsule1.Start;
        Vector2 v2 = capsule2.Start - capsule1.End;
        Vector2 v3 = capsule2.End - capsule1.End;

        //squared distances
        float d0 = Vector2.Dot(v0, v0);
        float d1 = Vector2.Dot(v1, v1);
        float d2 = Vector2.Dot(v2, v2);
        float d3 = Vector2.Dot(v3, v3);

        //best potential endpoint on capsule1
        Vector2 bestA;
        if (d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1)
            bestA = capsule1.End;
        else
            bestA = capsule1.Start;

        //select point on capsule1 nearest to best potential endpoint on capsule2, and vice-versa
        Vector2 bestB = Collisions.ClosestPointOnLine(capsule2.Start, capsule2.End, bestA);
        bestA = Collisions.ClosestPointOnLine(capsule1.Start, capsule1.End, bestB);

        return CircleCircle(bestA, capsule1.Radius, bestB, capsule2.Radius, ref result);
    }

    public static bool CapsulePolygon(in CapsuleShape capsule, in PolygonShape polygon, ref CollisionManifold result)
    {
        float retDepth = float.MinValue;
        Vector2 retNormal = Vector2.Zero;
        for (int i = 0; i < polygon.TransformedVertices.Length; i++)
        {
            Vector2 closestPoint = Collisions.ClosestPointOnLine(capsule.Start, capsule.End, polygon.TransformedVertices[i]);
            bool ret = CirclePolygon(polygon, closestPoint, capsule.Radius, ref result);
            if (ret && retDepth < result.depth)
            {
                retDepth = result.depth;
                retNormal = result.normal;
                result.contactPoint1 = closestPoint;
            }
        }
        result.normal = retNormal;
        result.depth = retDepth;
        result.contactPoint1 += result.normal * capsule.Radius;

        return retDepth != float.MinValue;
    }
    #endregion
    #region Polygon
    public static bool PolygonPolygon(PolygonShape poly1, PolygonShape poly2, ref CollisionManifold result)
    {
        result.depth = float.MaxValue;

        for (int i = 0; i < poly1.TransformedVertices.Length; i++)
        {
            Vector2 va = poly1.TransformedVertices[i];
            Vector2 vb = poly1.TransformedVertices[(i + 1) % poly1.TransformedVertices.Length];

            Vector2 edge = vb - va;
            Vector2 axis = new Vector2(-edge.Y, edge.X);
            axis = Lib.Math.Normalize(axis);

            ProjectVertices(poly1.TransformedVertices, axis, out float minA, out float maxA);
            ProjectVertices(poly2.TransformedVertices, axis, out float minB, out float maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            float axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < result.depth)
            {
                result.depth = axisDepth;
                result.normal = axis;
            }
        }

        for (int i = 0; i < poly2.TransformedVertices.Length; i++)
        {
            Vector2 va = poly2.TransformedVertices[i];
            Vector2 vb = poly2.TransformedVertices[(i + 1) % poly2.TransformedVertices.Length];

            Vector2 edge = vb - va;
            Vector2 axis = new Vector2(-edge.Y, edge.X);
            axis = Lib.Math.Normalize(axis);

            ProjectVertices(poly1.TransformedVertices, axis, out float minA, out float maxA);
            ProjectVertices(poly2.TransformedVertices, axis, out float minB, out float maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            float axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < result.depth)
            {
                result.depth = axisDepth;
                result.normal = axis;
            }
        }

        Vector2 direction = poly2.Transform.Position - poly1.Transform.Position;

        if (Vector2.Dot(direction, result.normal) < 0f)
        {
            result.normal = -result.normal;
        }
        ColliderShapeUtility.FindContactPoints(poly1.TransformedVertices, poly2.TransformedVertices, out result.contactPoint1, out Vector2 contact2, out int count);
        if (count > 1)
            result.contactPoint2 = contact2;

        return true;
    }

    private static void ProjectVertices(Vector2[] vertices, Vector2 axis, out float min, out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v = vertices[i];
            float proj = Vector2.Dot(v, axis);

            if (proj < min) { min = proj; }
            if (proj > max) { max = proj; }
        }
    }

    #endregion
}
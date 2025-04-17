using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo.Lib;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Dynamics.Shapes;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Rubedo.Physics2D.Math;

internal static class PhysicsCollisions
{
    #region Collide
    public static bool Collide(Shape a, Shape b, Manifold m)
    {
        switch (a.type)
        {
            case Collision.Shapes.ShapeType.Circle:
                Circle circle = (Circle)a;
                switch (b.type)
                {
                    case Collision.Shapes.ShapeType.Circle:
                        return CircleToCircle(m, circle, (Circle)b);
                    case Collision.Shapes.ShapeType.Capsule:
                        return CircleToCapsule(m, circle, (Capsule)b);
                    case Collision.Shapes.ShapeType.Box:
                        return CircleToBox(m, circle, (Box)b);
                    case Collision.Shapes.ShapeType.Polygon:
                        return CircleToPolygon(m, circle, (Polygon)b);
                }
                return false;
            case Collision.Shapes.ShapeType.Capsule:
                Capsule capsule = (Capsule)a;
                switch (b.type)
                {
                    case Collision.Shapes.ShapeType.Circle:
                        return CapsuleToCircle(m, capsule, (Circle)b);
                    case Collision.Shapes.ShapeType.Capsule:
                        return CapsuleToCapsule(m, capsule, (Capsule)b);
                    case Collision.Shapes.ShapeType.Box:
                        return CapsuleToBox(m, capsule, (Box)b);
                    case Collision.Shapes.ShapeType.Polygon:
                        return CapsuleToPolygon(m, capsule, (Polygon)b);
                }
                return false;
            case Collision.Shapes.ShapeType.Box:
                Box box = (Box)a;
                switch (b.type)
                {
                    case Collision.Shapes.ShapeType.Circle:
                        return BoxToCircle(m, box, (Circle)b);
                    case Collision.Shapes.ShapeType.Capsule:
                        return BoxToCapsule(m, box, (Capsule)b);
                    case Collision.Shapes.ShapeType.Box:
                        return BoxToBox(m, box, (Box)b);
                    case Collision.Shapes.ShapeType.Polygon:
                        return BoxToPolygon(m, box, (Polygon)b);
                }
                return false;
            case Collision.Shapes.ShapeType.Polygon:
                Polygon poly = (Polygon)a;
                switch (b.type)
                {
                    case Collision.Shapes.ShapeType.Circle:
                        return PolygonToCircle(m, poly, (Circle)b);
                    case Collision.Shapes.ShapeType.Capsule:
                        return PolygonToCapsule(m, poly, (Capsule)b);
                    case Collision.Shapes.ShapeType.Box:
                        return PolygonToBox(m, poly, (Box)b);
                    case Collision.Shapes.ShapeType.Polygon:
                        return PolygonToPolygon(m, poly, (Polygon)b);
                }
                return false;
        }
        return false;
    }
    #endregion

    #region Circle
    public static bool CircleToCircle(Manifold m, Circle circleA, Circle circleB)
    {
        return CircleToCircle(m, circleA.transform.Position, circleA.radius * Lib.Math.Max(circleA.transform.Scale),
            circleB.transform.Position, circleB.radius * Lib.Math.Max(circleB.transform.Scale));
    }
    public static bool CircleToCircle(Manifold m, Vector2 circPos1, float circRad1, Vector2 circPos2, float circRad2)
    {
        m.contactCount = 0;

        Vector2 n = circPos2 - circPos1;
        float r = circRad1 + circRad2;
        float len = n.LengthSquared();

        // If distance greater than sum of radii then exit
        if (len > r * r)
            return false;

        Contact contact;

        if (len != 0)
        {
            float d = MathF.Sqrt(len);
            m.normal = n / d;
            contact = new Contact(m.normal * circRad1 + circPos1);
            contact.penetration = r - d;
        }
        else
        {
            contact = new Contact(circPos1);
            // Circles are concentric, take a consistent value
            contact.penetration = circRad1;
            m.normal = Vector2.UnitY;
        }

        m.Update(1, contact);
        return true;
    }
    public static bool CircleToBox(in Manifold m, Circle circle, Box box)
    {
        //TODO: make a fast as fuq version for unrotated boxes.
        return CircleToPolygon(m, circle, box);
    }
    public static bool CircleToCapsule(Manifold m, Circle circle, Capsule capsule)
    {
        //we pick the "most relevant circle" to do collision with.
        //Basically, we find the closest point to our shape on the capsule line
        //and do a circle collision there.
        Vector2 circlePos = circle.transform.Position;
        capsule.TransformPoints();
        ShapeUtility.ClosestPointOnLine(ref capsule.transStart, ref capsule.transEnd, ref circlePos, out Vector2 closestPoint);
        return CircleToCircle(m, circlePos, circle.radius * Lib.Math.Max(circle.transform.Scale), closestPoint, capsule.transRadius);
    }
    public static bool CircleToPolygon(Manifold m, Circle circle, Polygon poly)
    {
        return CircleToPolygon(m, circle.transform.Position, circle.radius * Lib.Math.Max(circle.transform.Scale), poly);
    }

    public static bool CircleToPolygonV1(Manifold m, Vector2 circPos, float radius, Polygon poly)
    {
        m.contactCount = 0;

        poly.TransformVertices();
        poly.TransformNormals();

        Vector2 axis;
        float axisDepth;
        float depth = float.MaxValue;
        float minA, maxA, minB, maxB;
        for (int i = 0; i < poly.VertexCount; i++)
        {
            ProjectPolygon(poly, ref poly.transformedNormals[i], out minA, out maxA);
            ProjectCircle(ref circPos, radius, ref poly.transformedNormals[i], out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
                return false;

            axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth)
            {
                depth = axisDepth;
                m.normal = poly.transformedNormals[i];
            }
        }

        ShapeUtility.GetClosestPointOnPolygon(poly, circPos, out _, out axis);

        ProjectPolygon(poly, ref axis, out minA, out maxA);
        ProjectCircle(ref circPos, radius, ref axis, out minB, out maxB);

        if (minA >= maxB || minB >= maxA)
            return false;

        axisDepth = MathF.Min(maxB - minA, maxA - minB);

        if (axisDepth < depth)
        {
            depth = axisDepth;
            m.normal = axis;
        }

        Vector2 direction = poly.transform.Position - circPos;

        if (Vector2.Dot(direction, m.normal) < 0f)
            m.normal = -m.normal;

        //get contact point on closest edge.
        axisDepth = float.MaxValue;
        Vector2 point = Vector2.Zero;
        for (int i = 0; i < poly.VertexCount; i++)
        {
            ShapeUtility.ClosestPointOnLine(ref poly.transformedVertices[i],
                    ref poly.transformedVertices[(i + 1) % poly.VertexCount], ref circPos, out Vector2 a);
            Vector2.DistanceSquared(ref circPos, ref a, out float dist);
            if (dist < axisDepth)
            {
                axisDepth = dist;
                point = a;
            }
        }
        if (axisDepth >= radius * radius)
            return false;

        Contact c = new Contact(point);

        c.penetration = depth;
        m.Update(1, c);
        return true;
    }

    private static void ProjectPolygon(Polygon poly, ref Vector2 axis, out float min, out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;

        for (int i = 0; i < poly.VertexCount; i++)
        {
            Vector2.Dot(ref poly.transformedVertices[i], ref axis, out float projection);

            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }
    }
    private static void ProjectCircle(ref Vector2 pos, float radius, ref Vector2 axis, out float min, out float max)
    {
        Vector2.Multiply(ref axis, radius, out Vector2 dir);

        Vector2.Add(ref pos, ref dir, out Vector2 p1);
        Vector2.Subtract(ref pos, ref dir, out Vector2 p2);

        Vector2.Dot(ref p1, ref axis, out min);
        Vector2.Dot(ref p2, ref axis, out max);

        if (min > max)
        {
            (max, min) = (min, max);
        }
    }
    
    public static bool CircleToPolygon(Manifold m, Vector2 circPos, float radius, Polygon poly)
    {
        m.contactCount = 0;
        poly.TransformVertices();
        poly.TransformNormals();

        float distanceSquared = float.MaxValue;
        Vector2 closestPoint = Vector2.Zero;

        bool isInside = false;
        for (int i = 0; i < poly.VertexCount; i++)
        {
            Vector2 a = poly.transformedVertices[i];
            Vector2 b = poly.transformedVertices[(i + 1) % poly.VertexCount];

            if (((a.Y > circPos.Y) != (b.Y > circPos.Y)) &&
               (circPos.X < (b.X - a.X) * (circPos.Y - a.Y) / (b.Y - a.Y) + a.X))
                isInside = !isInside;

            ShapeUtility.ClosestPointOnLine(ref a, ref b, ref circPos, out Vector2 closest);
            Vector2.DistanceSquared(ref circPos, ref closest, out float tempDistanceSquared);

            if (tempDistanceSquared < distanceSquared)
            {
                distanceSquared = tempDistanceSquared;
                closestPoint = closest;
            }
        }

        if (isInside)
        { //the circle center is contained in the polygon, yeet it out.
            m.normal = -(closestPoint - circPos);
            MathV.Normalize(ref m.normal);

            MathV.MulAdd(ref circPos, ref m.normal, radius, out Vector2 pos);
            Contact c = new Contact(pos);
            c.penetration = MathF.Sqrt(distanceSquared) + radius;
            m.Update(1, c);
            return true;
        }

        float sqrRad = radius * radius;
        if (distanceSquared < sqrRad)
        {
            m.normal = (closestPoint - circPos);
            MathV.Normalize(ref m.normal);

            MathV.MulAdd(ref circPos, ref m.normal, radius, out Vector2 pos);
            Contact c = new Contact(pos);
            c.penetration = radius - MathF.Sqrt(distanceSquared);
            m.Update(1, c);
            return true;
        }
        return false;
    }

    #endregion
    #region Capsule
    public static bool CapsuleToCircle(Manifold m, Capsule capsule, Circle circle)
    {
        bool ret = CircleToCapsule(m, circle, capsule);
        m.normal = -m.normal;
        return ret;
    }
    public static bool CapsuleToBox(Manifold m, Capsule capsule, Box box)
    {
        return CapsuleToPolygon(m, capsule, box);
    }
    public static bool CapsuleToCapsule(in Manifold m, in Capsule capsule1, in Capsule capsule2)
    {
        m.contactCount = 0;

        capsule1.TransformPoints();
        capsule2.TransformPoints();

        //vectors between line endpoints
        Vector2 v0 = capsule2.transStart - capsule1.transStart;
        Vector2 v1 = capsule2.transEnd - capsule1.transStart;
        Vector2 v2 = capsule2.transStart - capsule1.transEnd;
        Vector2 v3 = capsule2.transEnd - capsule1.transEnd;

        //squared distances
        Vector2.Dot(ref v0, ref v0, out float d0);
        Vector2.Dot(ref v1, ref v1, out float d1);
        Vector2.Dot(ref v2, ref v2, out float d2);
        Vector2.Dot(ref v3, ref v3, out float d3);

        //best potential endpoint on capsule1
        Vector2 bestA;
        if (d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1)
            bestA = capsule1.transEnd;
        else
            bestA = capsule1.transStart;

        //select point on capsule1 nearest to best potential endpoint on capsule2, and vice-versa
        ShapeUtility.ClosestPointOnLine(ref capsule2.transStart, ref capsule2.transEnd, ref bestA, out Vector2 bestB);
        ShapeUtility.ClosestPointOnLine(ref capsule1.transStart, ref capsule1.transEnd, ref bestB, out bestA);
        ShapeUtility.ClosestPointOnLine(ref capsule2.transStart, ref capsule2.transEnd, ref bestA, out bestB); //must correct for far-away endpoints.
        if (bestA == bestB)
        {   //if they're equal, that means our capsule lines are intersecting
            //which means we need to get the perpendicular normal to the smallest squared distance.
            Contact contact = new Contact(bestA);//arbitrary contact point.
            float min = MathF.Min(MathF.Min(d0, d1), MathF.Min(d2, d3));
            if (min == d0)
            {
                contact.penetration = MathF.Sqrt(d0);
                m.normal = v0;
            }
            else if (min == d1)
            {
                contact.penetration = MathF.Sqrt(d1);
                m.normal = v1;
            }
            else if (min == d2)
            {
                contact.penetration = MathF.Sqrt(d2);
                m.normal = v2;
            }
            else
            {
                contact.penetration = MathF.Sqrt(d3);
                m.normal = v3;
            }
            MathV.Normalize(ref m.normal);
            MathV.Left(ref m.normal, out m.normal);
            m.Update(1, contact);
            return true;
        }
        
        return CircleToCircle(m, bestA, capsule1.transRadius, bestB, capsule2.transRadius);
    }
    public static bool CapsuleToPolygon(in Manifold m, in Capsule capsule, in Polygon poly)
    {
        m.contactCount = 0;
        capsule.TransformPoints();
        poly.TransformVertices();
        poly.TransformNormals();
        Vector2 polyPos = poly.transform.Position;

        ref Vector2 start = ref capsule.transStart;
        ref Vector2 end = ref capsule.transEnd;
        ref float radius = ref capsule.transRadius;

        ShapeUtility.ClosestPointOnLine(ref start, ref end, ref polyPos, out Vector2 center);

        float distanceSquared = float.MaxValue;
        Vector2 closestPoint = Vector2.Zero;
        Vector2 closest;
        int edgeVert = 0;

        bool isEndInside = false;
        bool isStartInside = false;
        for (int i = 0; i < poly.VertexCount; i++)
        {
            Vector2 a = poly.transformedVertices[i];
            Vector2 b = poly.transformedVertices[(i + 1) % poly.VertexCount];

            if (((a.Y > start.Y) != (b.Y > start.Y)) &&
               (start.X < (b.X - a.X) * (start.Y - a.Y) / (b.Y - a.Y) + a.X))
            {
                isStartInside = !isStartInside;
            }
            if (((a.Y > end.Y) != (b.Y > end.Y)) &&
               (end.X < (b.X - a.X) * (end.Y - a.Y) / (b.Y - a.Y) + a.X))
                isEndInside = !isEndInside;

            ShapeUtility.ClosestPointOnLine(ref a, ref b, ref center, out closest);
            Vector2.DistanceSquared(ref center, ref closest, out float tempDistanceSquared);

            if (tempDistanceSquared < distanceSquared)
            {
                distanceSquared = tempDistanceSquared;
                closestPoint = closest;
                edgeVert = i;
            }
        }

        if (isStartInside && isEndInside)
        { //the capsule is contained in the polygon, yeet it out.

            float dist = MathF.Sqrt(distanceSquared);
            //Vector2 point = poly.transform.LocalToWorldPosition(closestPoint);
            //center = poly.transform.LocalToWorldPosition(center);
            m.normal = -(closestPoint - center);
            MathV.Normalize(ref m.normal);

            MathV.MulSub(ref center, ref m.normal, radius, out Vector2 pos);
            Contact c = new Contact(pos);
            c.penetration = dist + radius;
            m.Update(1, c);
            return true;
        }

        //we essentially turn this into a line - capsule collision, but with at most 2 contact points.
        Vector2 v1 = poly.transformedVertices[edgeVert];
        Vector2 v2 = poly.transformedVertices[(edgeVert + 1) % poly.VertexCount];

        ShapeUtility.ClosestPointOnLine(ref start, ref end, ref closestPoint, out Vector2 v3Point);
        ShapeUtility.ClosestPointOnLine(ref v1, ref v2, ref v3Point, out Vector2 v3); //v3 is the best point on the edge for circle collisions.

        m.normal = v3 - v3Point;
        MathV.Normalize(ref m.normal);
        if (isStartInside || isEndInside)
            m.normal = -m.normal;

        float sqrRad = radius * radius;
        if (v3Point == start || v3Point == end)
        { //the midpoint is on a cap, do a regular circle collision.
            Vector2.DistanceSquared(ref v3Point, ref v3, out float dist);
            if (dist < sqrRad)
            {
                MathV.MulAdd(ref center, ref m.normal, radius, out Vector2 pos);
                Contact c = new Contact(pos);
                c.penetration = radius - MathF.Sqrt(dist);
                m.Update(1, c);
                return true;
            }
            return false;
        }
        Vector2[] face = new Vector2[] { v1, v2 };
        //clip our edge face to fit on the capsule line

        Vector2 capsuleNormal = end - start;
        MathV.Normalize(ref capsuleNormal);

        Vector2.Dot(ref capsuleNormal, ref start, out float negSide);
        Vector2.Dot(ref capsuleNormal, ref end, out float posSide);

        // Due to floating point error, possible to not have required points
        if (Clip(-capsuleNormal, -negSide, ref face) < 2)
            return false;

        if (Clip(capsuleNormal, posSide, ref face) < 2)
            return false;

        v1 = face[0];
        v2 = face[1];

        ShapeUtility.ClosestPointOnLine(ref start, ref end, ref v1, out Vector2 v1Point);
        ShapeUtility.ClosestPointOnLine(ref start, ref end, ref v2, out Vector2 v2Point);

        //discard the greatest distance.
        float dist1 = Vector2.DistanceSquared(v1Point, v1);
        float dist2 = Vector2.DistanceSquared(v2Point, v2);
        float dist3 = Vector2.DistanceSquared(v3Point, v3) + Lib.Math.EPSILON;

        //midpoint discarded preferably if points are parallel to capsule plane,
        //or both edge points are colliding.
        bool useDist1 = (dist1 < dist2 || dist1 <= dist3) && dist1 < sqrRad;
        bool useDist2 = (dist2 < dist1 || dist2 <= dist3) && dist2 < sqrRad;
        bool useDist3 = (dist3 < dist1 || dist3 < dist2 || (!useDist1 || !useDist2)) && dist3 - Lib.Math.EPSILON < sqrRad;

        if (!useDist1 && !useDist2 && !useDist3)
            return false; //no contact

        Contact[] contacts = new Contact[2];
        int cp = 0;
        if (useDist1)
        {
            contacts[cp] = new Contact(v1);
            contacts[cp].penetration = radius - MathF.Sqrt(dist1);
            ++cp;
        }

        if (useDist2)
        {
            if (contacts[0] == null || !MathV.FastEquals(ref contacts[0].position, ref v2))
            {
                contacts[cp] = new Contact(v2);
                contacts[cp].penetration = radius - MathF.Sqrt(dist2);
                ++cp;
            }
        }
        if (useDist3)
        {
            if (contacts[0] == null || !MathV.FastEquals(ref contacts[0].position, ref v3))
            {
                contacts[cp] = new Contact(v3);
                contacts[cp].penetration = radius - MathF.Sqrt(dist3 - Lib.Math.EPSILON);
                ++cp;
            }
        }
        if (cp == 0)
            return false;

        m.contactCount = cp;
        m.Update(cp, contacts);
        return cp > 0;
    }
    #endregion
    #region Box
    public static bool BoxToCircle(Manifold m, Box a, Circle b)
    {
        //Just switching the input so that we can pass it to the function above
        bool ret = CircleToBox(m, b, a);
        //Make sure that normal always points from A to B
        m.normal = -m.normal;
        return ret;
    }
    public static bool BoxToCapsule(Manifold m, Box a, Capsule b)
    {
        return PolygonToCapsule(m, a, b);
    }
    public static bool BoxToBox(Manifold m, Box a, Box b)
    {
        //TODO: Make a fast as fuq version for unrotated boxes.
        return PolygonToPolygon(m, a, b);
    }
    public static bool BoxToPolygon(Manifold m, Box a, Polygon b)
    {
        return PolygonToPolygon(m, a, b);
    }
    #endregion
    #region Polygon
    public static bool PolygonToCircle(Manifold m, Polygon a, Circle b)
    {
        //Just switching the input so that we can pass it to the function above
        bool ret = CircleToPolygon(m, b, a);
        //Make sure that normal always points from A to B
        m.normal = -m.normal;
        return ret;
    }
    public static bool PolygonToCapsule(Manifold m, Polygon a, Capsule b)
    {
        //Just switching the input so that we can pass it to the function above
        bool ret = CapsuleToPolygon(m, b, a);
        //Make sure that normal always points from A to B
        m.normal = -m.normal;
        return ret;
    }
    public static bool PolygonToBox(Manifold m, Polygon a, Box b)
    {
        return PolygonToPolygon(m, a, b);
    }

    public static bool PolygonToPolygon(Manifold m, Polygon polyA, Polygon polyB)
    {
        m.contactCount = 0;
        //make sure out polygons are updated.
        polyA.TransformVertices();
        polyA.TransformNormals();
        polyB.TransformVertices();
        polyB.TransformNormals();

        //Check for separating axis on A's normals
        int faceA;
        float penetrationA = FindLeastPenetrationAxis(polyA, polyB, out faceA);

        if (penetrationA >= 0) 
            return false;
        //Check for separating axis on B's normals
        int faceB;
        float penetrationB = FindLeastPenetrationAxis(polyB, polyA, out faceB);

        if (penetrationB >= 0)
            return false;
        int referenceIndex;
        //Make sure we always point from A to B for consistent results
        bool flip;

        Polygon refPoly, incPoly;

        if (Lib.Math.BiasGreaterThan(penetrationA, penetrationB))
        {
            refPoly = polyA;
            incPoly = polyB;
            referenceIndex = faceA;
            flip = false;
        }
        else
        {
            refPoly = polyB;
            incPoly = polyA;
            referenceIndex = faceB;
            flip = true;
        }

        Vector2[] incidentFace;
        GetIncidentFace(out incidentFace, refPoly, incPoly, referenceIndex);

        //Set up vertices
        Vector2 v1 = refPoly.transformedVertices[referenceIndex];
        Vector2 v2 = refPoly.transformedVertices[(referenceIndex + 1) % refPoly.VertexCount];

        Vector2 refFaceNormal = refPoly.transformedNormals[referenceIndex];
        MathV.Left(ref refFaceNormal, out Vector2 sidePlaneNormal);

        Vector2.Dot(ref refFaceNormal, ref v1, out float refC);
        Vector2.Dot(ref sidePlaneNormal, ref v1, out float negSide);
        Vector2.Dot(ref sidePlaneNormal, ref v2, out float posSide);

        // Due to floating point error, possible to not have required points
        if (Clip(-sidePlaneNormal, -negSide, ref incidentFace) < 2)
            return false;

        if (Clip(sidePlaneNormal, posSide, ref incidentFace) < 2)
            return false;

        m.normal = flip ? -refFaceNormal : refFaceNormal;

        Contact[] contacts = new Contact[2];

        // Keep points behind reference face
        int cp = 0; // clipped points behind reference face
        for (int i = 0; i < 2; i++)
        {
            Vector2.Dot(ref refFaceNormal, ref incidentFace[i], out float separation);
            separation -= refC;
            if (separation <= 0.0f)
            {
                contacts[cp] = new Contact(incidentFace[i]);
                contacts[cp].penetration = -separation;
                ++cp;
            }
        }

        m.contactCount = cp;
        m.Update(cp, contacts);
        return true;
    }

    private static float FindLeastPenetrationAxis(Polygon a, Polygon b, out int face)
    {
        float bestDistance = float.MinValue;
        face = 0;

        Vector2 n, neg;
        for (int i = 0; i < a.VertexCount; i++)
        {
            //Get world space normal of A's face
            n = a.transformedNormals[i];
            neg = -n;

            //Get furthest point in negative normal direction
            Vector2 s = Vector2.Zero;
            float maxProjection = float.MinValue;

            for (int z = 0; z < b.VertexCount; z++)
            {
                Vector2 vertex = b.transformedVertices[z];
                Vector2.Dot(ref vertex, ref neg, out float projection);

                //If vertex is furthest, projection is greatest
                if (projection > maxProjection)
                {
                    maxProjection = projection;
                    s = vertex;
                }
            }

            //Get vertex on A's face
            Vector2 v = s - a.transformedVertices[i];

            //Find penetration distance
            Vector2.Dot(ref n, ref v, out float d);

            //Store greatest distance
            if (d > bestDistance)
            {
                bestDistance = d;
                face = i;
                if (bestDistance >= 0)
                    return bestDistance; //early out, we're not colliding.
            }
        }

        return bestDistance;
    }

    private static void GetIncidentFace(out Vector2[] face,
        Polygon refP, Polygon incP, int referenceIndex)
    {
        face = new Vector2[2];
        //Retrieve the reference normal
        Vector2 n = refP.transformedNormals[referenceIndex];

        //Find incident face whose normal is most perpendicular to the reference normal
        int incidentFace = -1;
        float minDot = float.MaxValue;
        for (int i = 0; i < incP.VertexCount; i++)
        {
            Vector2.Dot(ref n, ref incP.transformedNormals[i], out float dot);
            if (dot < minDot)
            {
                minDot = dot;
                incidentFace = i;
            }
        }

        //Get world space face
        face[0] = incP.transformedVertices[incidentFace];
        face[1] = incP.transformedVertices[(incidentFace + 1) % incP.VertexCount];
    }

    private static int Clip(Vector2 n, float c, ref Vector2[] face)
    {
        int clippedPoints = 0;
        Vector2[] res = { face[0], face[1] };

        //Get distance to line (ax + by = -c, n = (a, b))
        float d1 = Vector2.Dot(n, face[0]) - c;
        float d2 = Vector2.Dot(n, face[1]) - c;

        //If behind plane, clip
        //Here we aren't actually clipping, but we are just incrementing the count
        if (d1 <= 0) res[clippedPoints++] = face[0];
        if (d2 <= 0) res[clippedPoints++] = face[1];

        //Check whether one of the points is ahead and other behind
        //(-) * (+) = (-)
        if (d1 * d2 < 0)
        {
            //Intersect
            float t = d1 / (d1 - d2);
            res[clippedPoints] = face[0] + (face[1] - face[0]) * t;
            clippedPoints++;
        }

        face[0] = res[0];
        face[1] = res[1];

        return clippedPoints;
    }
    #endregion
}

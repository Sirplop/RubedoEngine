using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Util;
using Rubedo.Render;
using System;

namespace Rubedo.Physics2D.Collision;

/// <summary>
/// I am NeoPhysicsCollisions, and this is my summary.
/// </summary>
internal static class PhysicsCollisions
{
    #region Helper
    public static bool Collide(in IShape shape1, in IShape shape2, ref Manifold result)
    {
        bool ret;
        switch (shape1.ShapeType)
        {
            case ShapeType.Circle:
                Circle circle1 = (Circle)shape1;
                switch (shape2.ShapeType)
                {
                    case ShapeType.Circle:
                        Circle circle2 = (Circle)shape2;
                        return CircleCircle(circle1, circle2, ref result);
                    case ShapeType.Box:
                        Box box2 = (Box)shape2;
                        return CirclePolygon(box2, circle1, ref result);
                    case ShapeType.Capsule:
                        Capsule capsule2 = (Capsule)shape2;
                        return CircleCapsule(circle1, capsule2, ref result);
                    case ShapeType.Polygon:
                        Polygon poly2 = (Polygon)shape2;
                        return CirclePolygon(poly2, circle1, ref result);
                }
                break;
            case ShapeType.Box:
                Box box1 = (Box)shape1;
                switch (shape2.ShapeType)
                {
                    case ShapeType.Circle:
                        Circle circle2 = (Circle)shape2;
                        ret = CirclePolygon(box1, circle2, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Box:
                        Box box2 = (Box)shape2;
                        return BoxBox(box1, box2, ref result);
                    case ShapeType.Capsule:
                        Capsule capsule2 = (Capsule)shape2;
                        ret = CapsulePolygon(capsule2, box1, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Polygon:
                        Polygon poly2 = (Polygon)shape2;
                        return PolygonPolygon(box1, poly2, ref result);
                }
                break;
            case ShapeType.Capsule:
                Capsule capsule1 = (Capsule)shape1;
                switch (shape2.ShapeType)
                {
                    case ShapeType.Circle:
                        Circle circle2 = (Circle)shape2;
                        ret = CircleCapsule(circle2, capsule1, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Box:
                        Box box2 = (Box)shape2;
                        return CapsulePolygon(capsule1, box2, ref result);
                    case ShapeType.Capsule:
                        Capsule capsule2 = (Capsule)shape2;
                        return CapsuleCapsule(capsule1, capsule2, ref result);
                    case ShapeType.Polygon:
                        Polygon poly2 = (Polygon)shape2;
                        return CapsulePolygon(capsule1, poly2, ref result);
                }
                break;
            case ShapeType.Polygon:
                Polygon poly1 = (Polygon)shape1;
                switch (shape2.ShapeType)
                {
                    case ShapeType.Circle:
                        Circle circle2 = (Circle)shape2;
                        ret = CirclePolygon(poly1, circle2, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Box:
                        Box box2 = (Box)shape2;
                        return PolygonPolygon(poly1, box2, ref result);
                    case ShapeType.Capsule:
                        Capsule capsule2 = (Capsule)shape2;
                        ret = CapsulePolygon(capsule2, poly1, ref result);
                        result.normal = -result.normal;
                        return ret;
                    case ShapeType.Polygon:
                        Polygon poly2 = (Polygon)shape2;
                        return PolygonPolygon(poly1, poly2, ref result);
                }
                break;
        }
        return false;
    }
    #endregion

    #region Circle
    public static bool CircleCircle(in Circle circle1, in Circle circle2, ref Manifold result)
    {
        return CircleCircle(circle1.transform.WorldPosition, circle1.radius * Lib.Math.Max(circle1.transform.WorldScale),
            circle2.transform.WorldPosition, circle2.radius * Lib.Math.Max(circle1.transform.WorldScale), ref result);
    }
    public static bool CircleCircle(Vector2 circleCenter1, float circleRadius1, Vector2 circleCenter2,
                                  float circleRadius2, ref Manifold result)
    {
        float sqrDist = Vector2.DistanceSquared(circleCenter1, circleCenter2);
        if (sqrDist >= (circleRadius1 + circleRadius2) * (circleRadius1 + circleRadius2))
        {
            return false;
        }
        Contact contact = result.GetContact(0);
        if (sqrDist == 0)
        { //consentric circles get arbitrary values.
            result.normal = Vector2.UnitY;
            contact.penetration = MathF.Max(circleRadius1, circleRadius2) * 0.5f;
            contact.position = result.normal * circleRadius1 + circleCenter1;
        }
        else
        {
            result.normal = Lib.Math.Normalize(circleCenter2 - circleCenter1);
            contact.penetration = circleRadius1 + circleRadius2 - MathF.Sqrt(sqrDist);
            contact.position = circleCenter1 + result.normal * circleRadius2;
        }
        result.contactCount = 1;
        return true;
    }
    public static bool CircleBox(Circle circle, Box box, ref Manifold result)
    {
        // Boxes that have undergone rotations must be calculated as polygons.
        if (box.transform.WorldRotation != 0)
            return CirclePolygon(box, circle, ref result);

        Vector2 circlePos = circle.transform.WorldPosition - box.transform.WorldPosition;
        
        Contact contact = result.GetContact(0);
        contact.position = ColliderShapeUtility.FindClosestPointOnBox(box, circlePos, out result.normal);

        // deal with circles whos center is in the box first since its cheaper to see if we are contained
        if (Collisions.BoxContainsPoint(box, circlePos))
        {
            result.normal = Lib.Math.Normalize(result.normal);
            //kinda janky, but it works
            Vector2 res = circlePos * result.normal - contact.position * result.normal;
            if (res.Y == 0)
                contact.penetration = MathF.Abs(res.X) + circle.radius;
            else
                contact.penetration = MathF.Abs(res.Y) + circle.radius;
            //result.normal = -result.normal;
            return true;
        }

        float sqrDistance = Vector2.DistanceSquared(contact.position, circlePos);

        // see if the point on the box is less than radius from the circle
        if (sqrDistance <= circle.radius * circle.radius)
        {
            result.normal = circlePos - contact.position;
            contact.penetration = result.normal.Length() - circle.radius;

            Lib.Math.Normalize(ref result.normal);
            result.contactCount = 1;

            return true;
        }

        return false;
    }
    public static bool CircleCapsule(Circle circle, Capsule capsule, ref Manifold result)
    {
        //we pick the "most relevant circle" to do collision with.
        //Basically, we find the closest point to our shape on the capsule line
        //and do a circle collision there.
        Vector2 circlePos = circle.transform.WorldPosition;
        capsule.GetTransformedPoints(out Vector2 start, out Vector2 end, out float radius);
        Vector2 closestPoint = Collisions.ClosestPointOnLine(start, end, circlePos);
        return CircleCircle(circlePos, circle.radius * Lib.Math.Max(circle.transform.WorldScale), closestPoint, radius, ref result);
    }
    public static bool CirclePolygon(in Polygon poly, in Circle circle, ref Manifold result)
    {
        return CirclePolygon(poly, circle.transform.WorldPosition, circle.radius * Lib.Math.Max(circle.transform.WorldScale), ref result);
    }
    public static bool CirclePolygon(Polygon poly, Vector2 circPos, float radius, ref Manifold m)
    {
        m.contactCount = 0;

        // Transform circle center to Polygon model space
        Vector2 center = poly.transform.WorldToLocalPosition(circPos);

        // Find edge with minimum penetration
        // Exact concept as using support points in Polygon vs Polygon
        float separation = float.MinValue;
        int faceNormal = 0;
        for (int i = 0; i < poly.VertexCount; ++i)
        {
            float s = Vector2.Dot(poly.normals[i], center - poly.vertices[i]);

            if (s > radius)
                return false;

            if (s > separation)
            {
                separation = s;
                faceNormal = i;
            }
        }

        // Grab face's vertices
        Vector2 v1 = poly.vertices[faceNormal];
        int i2 = faceNormal + 1 < poly.VertexCount ? faceNormal + 1 : 0;
        Vector2 v2 = poly.vertices[i2];

        Contact c = m.GetContact(0);

        // Check to see if center is within polygon
        if (separation < float.Epsilon)
        {
            m.normal = -poly.transform.LocalToWorldDirection(poly.normals[faceNormal]);
            c.position = m.normal * radius + circPos;
            c.penetration = radius;
            m.contactCount = 1;
            return true;
        }

        // Determine which voronoi region of the edge center of circle lies within
        float dot1 = Vector2.Dot(center - v1, v2 - v1);
        float dot2 = Vector2.Dot(center - v2, v1 - v2);
        c.penetration = radius - separation;

        // Closest to v1
        if (dot1 <= 0.0f)
        {
            if (Vector2.DistanceSquared(center, v1) > radius * radius)
                return false;

            Vector2 n = v1 - center;
            n = poly.transform.LocalToWorldDirection(n);
            n.Normalize();
            m.normal = n;

            v1 = poly.transform.LocalToWorldPosition(v1);
            c.position = v1;
            m.contactCount = 1;
        }

        // Closest to v2
        else if (dot2 <= 0.0f)
        {
            if (Vector2.DistanceSquared(center, v2) > radius * radius)
                return false;

            Vector2 n = v2 - center;
            v2 = poly.transform.LocalToWorldPosition(v2);
            c.position = v2;
            m.contactCount = 1;

            n = poly.transform.LocalToWorldDirection(n);
            n.Normalize();
            m.normal = n;
        }

        // Closest to face
        else
        {
            Vector2 n = poly.normals[faceNormal];
            if (Vector2.Dot(center - v1, n) > radius)
                return false;

            n = poly.transform.LocalToWorldDirection(n);
            m.normal = -n;
            c.position = m.normal * radius + circPos;
            m.contactCount = 1;
        }
        return true;
    }
    #endregion
    #region Box
    public static bool BoxBox(in Box box1, in Box box2, ref Manifold result)
    {
        //TODO: Find a box-box collider. It should be SUPER fast.
        return PolygonPolygon(box1, box2, ref result);
    }
    #endregion
    #region Capsule
    public static bool CapsuleCapsule(in Capsule capsule1, in Capsule capsule2, ref Manifold result)
    {
        capsule1.GetTransformedPoints(out Vector2 s1, out Vector2 e1, out float rad1);
        capsule2.GetTransformedPoints(out Vector2 s2, out Vector2 e2, out float rad2);

        //vectors between line endpoints
        Vector2 v0 = s2 - s1;
        Vector2 v1 = e2 - s1;
        Vector2 v2 = s2 - e1;
        Vector2 v3 = e2 - e1;

        //squared distances
        float d0 = Vector2.Dot(v0, v0);
        float d1 = Vector2.Dot(v1, v1);
        float d2 = Vector2.Dot(v2, v2);
        float d3 = Vector2.Dot(v3, v3);

        //best potential endpoint on capsule1
        Vector2 bestA;
        if (d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1)
            bestA = e1;
        else
            bestA = s1;

        //select point on capsule1 nearest to best potential endpoint on capsule2, and vice-versa
        Vector2 bestB = Collisions.ClosestPointOnLine(s2, e2, bestA);
        bestA = Collisions.ClosestPointOnLine(s1, s2, bestB);
        if (bestA == bestB)
        {   //if they're equal, that means our capsule lines are intersecting
            //which means we need to get the perpendicular normal to the smallest squared distance.
            Contact contact = result.GetContact(0);
            contact.position = bestA; //arbitrary contact point.
            float min = MathF.Min(MathF.Min(d0, d1), MathF.Min(d2, d3));
            if (min == d0)
            {
                contact.penetration = MathF.Sqrt(d0);
                result.normal = Lib.Math.Left(Lib.Math.Normalize(v0));
            }
            else if (min == d1)
            {
                contact.penetration = MathF.Sqrt(d1);
                result.normal = Lib.Math.Left(Lib.Math.Normalize(v1));
            }
            else if (min == d2)
            {
                contact.penetration = MathF.Sqrt(d2);
                result.normal = Lib.Math.Left(Lib.Math.Normalize(v2));
            }
            else
            {
                contact.penetration = MathF.Sqrt(d3);
                result.normal = Lib.Math.Left(Lib.Math.Normalize(v3));
            }
            return true;
        }

        bool ret = CircleCircle(bestA, rad1, bestB, rad2, ref result);
        if (ret)
        {
            //need to fix contact point.
            result.GetContact(0).position = bestA + result.normal * rad1;
            return true;
        }
        return false;
    }

    public static bool CapsulePolygon(in Capsule capsule, in Polygon polygon, ref Manifold result)
    {
        capsule.GetTransformedPoints(out Vector2 start, out Vector2 end, out float radius);

        // Transform the capsule into the polygon's local space
        start = polygon.transform.WorldToLocalPosition(start);
        end = polygon.transform.WorldToLocalPosition(end);

        float retDepth = float.MinValue;
        Vector2 retNormal = Vector2.Zero;
        Vector2 point = Vector2.Zero;
        Contact contact = result.GetContact(0);
        for (int i = 0; i < polygon.vertices.Length; i++)
        {
            Vector2 closestPoint = Collisions.ClosestPointOnLine(start, end, polygon.vertices[i]);
            bool ret = CirclePolygon(polygon, closestPoint, radius, ref result);
            if (ret && retDepth < contact.penetration)
            {
                retDepth = contact.penetration;
                retNormal = result.normal;
                point = closestPoint;
            }
        }
        result.normal = retNormal;
        contact.penetration = retDepth;
        contact.position = polygon.transform.LocalToWorldPosition(point);

        return retDepth != float.MinValue;
    }
    #endregion
    #region Polygon
    public static bool PolygonPolygon(in Polygon poly1, in Polygon poly2, ref Manifold result)
    {
        result.contactCount = 0;

        //Check for separating axis on A's normals
        int faceA;
        float penetrationA = FindLeastPenetrationAxis(poly1, poly2, out faceA);

        if (penetrationA >= 0)
            return false;

        //Check for separating axis on B's normals
        int faceB;
        float penetrationB = FindLeastPenetrationAxis(poly2, poly1, out faceB);

        if (penetrationB >= 0)
            return false;

        int referenceIndex;
        //Make sure we always point from A to B for consistent results
        bool flip;

        Polygon refPoly, incPoly;

        if (Lib.Math.BiasGreaterThan(penetrationA, penetrationB))
        {
            refPoly = poly1;
            incPoly = poly2;
            referenceIndex = faceA;
            flip = false;
        }
        else
        {
            refPoly = poly2;
            incPoly = poly1;
            referenceIndex = faceB;
            flip = true;
        }

        Vector2[] incidentFace;
        GetIncidentFace(out incidentFace, refPoly, incPoly, referenceIndex);

        //Set up vertices
        Vector2 v1 = refPoly.vertices[referenceIndex];
        Vector2 v2 = refPoly.vertices[(referenceIndex + 1) % refPoly.VertexCount];

        //Transform to world space
        v1 = refPoly.transform.LocalToWorldPosition(v1);
        v2 = refPoly.transform.LocalToWorldPosition(v2);

        Vector2 sidePlaneNormal = v2 - v1;
        sidePlaneNormal.Normalize();

        Vector2 refFaceNormal = new Vector2(sidePlaneNormal.Y, -sidePlaneNormal.X);

        float refC = Vector2.Dot(refFaceNormal, v1);
        float negSide = -Vector2.Dot(sidePlaneNormal, v1);
        float posSide = Vector2.Dot(sidePlaneNormal, v2);

        // Due to floating point error, possible to not have required points
        if (Clip(-sidePlaneNormal, negSide, ref incidentFace) < 2)
            return false;

        if (Clip(sidePlaneNormal, posSide, ref incidentFace) < 2)
            return false;

        result.normal = flip ? -refFaceNormal : refFaceNormal;

        // Keep points behind reference face
        int cp = 0; // clipped points behind reference face
        for (int i = 0; i < 2; i++)
        {
            float separation = Vector2.Dot(refFaceNormal, incidentFace[i]) - refC;
            if (separation <= 0.0f)
            {
                Contact c = result.GetContact(cp);
                c.position = incidentFace[i];
                c.penetration = -separation;
                ++cp;
            }
        }

        result.contactCount = cp;
        return true;
    }

    private static float FindLeastPenetrationAxis(Polygon a, Polygon b, out int face)
    {
        float bestDistance = float.MinValue;
        face = 0;

        for (int i = 0; i < a.VertexCount; i++)
        {
            //Get world space normal of A's face
            Vector2 n = a.normals[i];
            Vector2 nW = a.transform.LocalToWorldDirection(n);

            //Retrieve normal in B's space
            n = b.transform.WorldToLocalDirection(nW);

            //Get furthest point in negative normal direction
            Vector2 s = b.GetSupportPoint(-n);

            //Get vertex on A's face, transform to B's space
            Vector2 v = a.vertices[i];
            v = a.transform.LocalToWorldPosition(v);
            v = b.transform.WorldToLocalPosition(v);

            //Find penetration distance
            float d = Vector2.Dot(s - v, n);

            //Store greatest distance
            if (d > bestDistance)
            {
                bestDistance = d;
                face = i;
            }
        }

        return bestDistance;
    }

    private static void GetIncidentFace(out Vector2[] face,
        Polygon refP, Polygon incP, int referenceIndex)
    {
        face = new Vector2[2];
        //Retrieve the reference normal
        Vector2 n = refP.normals[referenceIndex];

        //Transform into incident poly's space
        n = refP.transform.LocalToWorldDirection(n);    //To world space
        n = incP.transform.WorldToLocalDirection(n);    //To incident poly's space

        //Find face whose normal is most normal (perpendicular) to the normal (oh...)
        int incidentFace = -1;
        float minDot = float.MaxValue;

        for (int i = 0; i < incP.VertexCount; i++)
        {
            float dot = Vector2.Dot(n, incP.normals[i]);
            if (dot < minDot)
            {
                minDot = dot;
                incidentFace = i;
            }
        }

        //Get world space face
        face[0] = incP.transform.LocalToWorldPosition(incP.vertices[incidentFace]);
        face[1] = incP.transform.LocalToWorldPosition(incP.vertices[(incidentFace + 1) % incP.VertexCount]);
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
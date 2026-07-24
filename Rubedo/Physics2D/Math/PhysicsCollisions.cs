using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Dynamics.Shapes;
using System;

namespace Rubedo.Physics2D.Math;

public static class PhysicsCollisions
{
    #region Collide
    public static bool Collide(Shape a, Shape b, Manifold m)
    {
        switch (a.type)
        {
            case ShapeType.Circle:
                Circle circle = (Circle)a;
                switch (b.type)
                {
                    case ShapeType.Circle:
                        return CircleToCircle(ref m, circle, (Circle)b);
                    case ShapeType.Capsule:
                        return CircleToCapsule(ref m, circle, (Capsule)b);
                    case ShapeType.Box:
                        return CircleToBox(ref m, circle, (Box)b);
                    case ShapeType.Polygon:
                        return CircleToPolygon(ref m, circle, (Polygon)b);
                    case ShapeType.Compound:
                        return CompoundToOther(ref m, (CompoundShape)b, a, false);
                }
                return false;
            case ShapeType.Capsule:
                Capsule capsule = (Capsule)a;
                switch (b.type)
                {
                    case ShapeType.Circle:
                        return CapsuleToCircle(ref m, capsule, (Circle)b);
                    case ShapeType.Capsule:
                        return CapsuleToCapsule(ref m, capsule, (Capsule)b);
                    case ShapeType.Box:
                        return CapsuleToBox(ref m, capsule, (Box)b);
                    case ShapeType.Polygon:
                        return CapsuleToPolygon(ref m, capsule, (Polygon)b);
                    case ShapeType.Compound:
                        return CompoundToOther(ref m, (CompoundShape)b, a, false);
                }
                return false;
            case ShapeType.Box:
                Box box = (Box)a;
                switch (b.type)
                {
                    case ShapeType.Circle:
                        return BoxToCircle(ref m, box, (Circle)b);
                    case ShapeType.Capsule:
                        return BoxToCapsule(ref m, box, (Capsule)b);
                    case ShapeType.Box:
                        return BoxToBox(ref m, box, (Box)b);
                    case ShapeType.Polygon:
                        return BoxToPolygon(ref m, box, (Polygon)b);
                    case ShapeType.Compound:
                        return CompoundToOther(ref m, (CompoundShape)b, a, false);
                }
                return false;
            case ShapeType.Polygon:
                Polygon poly = (Polygon)a;
                switch (b.type)
                {
                    case ShapeType.Circle:
                        return PolygonToCircle(ref m, poly, (Circle)b);
                    case ShapeType.Capsule:
                        return PolygonToCapsule(ref m, poly, (Capsule)b);
                    case ShapeType.Box:
                        return PolygonToBox(ref m, poly, (Box)b);
                    case ShapeType.Polygon:
                        return PolygonToPolygon(ref m, poly, (Polygon)b);
                    case ShapeType.Compound:
                        return CompoundToOther(ref m, (CompoundShape)b, a, false);
                }
                return false;
            case ShapeType.Compound:
                // CompoundToOther itself checks whether b is also a Compound and
                // redirects to the compound-vs-compound path, so a single call
                // here handles every case on the "b" side.
                return CompoundToOther(ref m, (CompoundShape)a, b, true);
        }
        return false;
    }
    #endregion

    #region Circle
    public static bool CircleToCircle(ref Manifold m, Circle circleA, Circle circleB)
    {
        return CircleToCircle(ref m, circleA.Transform.Position, circleA.radius * Lib.MathV.Max(circleA.Transform.Scale),
            circleB.Transform.Position, circleB.radius * Lib.MathV.Max(circleB.Transform.Scale));
    }
    public static bool CircleToCircle(ref Manifold m, Vector2 circPos1, float circRad1, Vector2 circPos2, float circRad2)
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

        m.Update(contact);
        return true;
    }
    public static bool CircleToBox(ref Manifold m, Circle circle, Box box)
    {
        //TODO: make a fast as fuq version for unrotated boxes.
        return CircleToPolygon(ref m, circle, box);
    }
    public static bool CircleToCapsule(ref Manifold m, Circle circle, Capsule capsule)
    {
        return CircleToCapsule(ref m, circle.Transform.Position, circle.radius * Lib.MathV.Max(circle.Transform.Scale), capsule);
    }

    /// <summary>
    /// Raw-position variant of CircleToCapsule, needed for compound circle children:
    /// their shared Transform.Position is the compound's center, not their own
    /// world position, so callers must supply the already-resolved world position.
    /// </summary>
    public static bool CircleToCapsule(ref Manifold m, Vector2 circPos, float radius, Capsule capsule)
    {
        //we pick the "most relevant circle" to do collision with.
        //Basically, we find the closest point to our shape on the capsule line
        //and do a circle collision there.
        capsule.TransformPoints();
        ShapeUtility.ClosestPointOnLine(ref capsule.transStart, ref capsule.transEnd, ref circPos, out Vector2 closestPoint);
        return CircleToCircle(ref m, circPos, radius, closestPoint, capsule.transRadius);
    }
    public static bool CircleToPolygon(ref Manifold m, Circle circle, Polygon poly)
    {
        return CircleToPolygon(ref m, circle.Transform.Position, circle.radius * Lib.MathV.Max(circle.Transform.Scale), poly);
    }

    public static bool CircleToPolygon(ref Manifold m, Vector2 circPos, float radius, Polygon poly)
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
            m.Update(c);
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
            m.Update(c);
            return true;
        }
        return false;
    }

    #endregion
    #region Capsule
    public static bool CapsuleToCircle(ref Manifold m, Capsule capsule, Circle circle)
    {
        bool ret = CircleToCapsule(ref m, circle, capsule);
        m.normal = -m.normal;
        return ret;
    }
    public static bool CapsuleToBox(ref Manifold m, Capsule capsule, Box box)
    {
        return CapsuleToPolygon(ref m, capsule, box);
    }
    public static bool CapsuleToCapsule(ref Manifold m, in Capsule capsule1, in Capsule capsule2)
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
            m.Update(contact);
            return true;
        }

        return CircleToCircle(ref m, bestA, capsule1.transRadius, bestB, capsule2.transRadius);
    }
    public static bool CapsuleToPolygon(ref Manifold m, in Capsule capsule, in Polygon poly)
    {
        m.contactCount = 0;
        capsule.TransformPoints();
        poly.TransformVertices();
        poly.TransformNormals();
        Vector2 polyPos = poly.Transform.Position;

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
            m.Update(c);
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
                m.Update(c);
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
            if (contacts[0] == null || contacts[0].position != v2)
            {
                contacts[cp] = new Contact(v2);
                contacts[cp].penetration = radius - MathF.Sqrt(dist2);
                ++cp;
            }
        }
        if (useDist3)
        {
            if (contacts[0] == null || contacts[0].position != v3)
            {
                contacts[cp] = new Contact(v3);
                contacts[cp].penetration = radius - MathF.Sqrt(dist3 - Lib.Math.EPSILON);
                ++cp;
            }
        }
        if (cp == 0)
            return false;
        else if (cp == 1)
            m.Update(contacts[0]);
        else
            m.Update(contacts[0], contacts[1]);

        return cp > 0;
    }
    #endregion
    #region Box
    public static bool BoxToCircle(ref Manifold m, Box a, Circle b)
    {
        //Just switching the input so that we can pass it to the function above
        bool ret = CircleToBox(ref m, b, a);
        //Make sure that normal always points from A to B
        m.normal = -m.normal;
        return ret;
    }
    public static bool BoxToCapsule(ref Manifold m, Box a, Capsule b)
    {
        return PolygonToCapsule(ref m, a, b);
    }
    public static bool BoxToBox(ref Manifold m, Box a, Box b)
    {
        //TODO: Make a fast as fuq version for unrotated boxes.
        return PolygonToPolygon(ref m, a, b);
    }
    public static bool BoxToPolygon(ref Manifold m, Box a, Polygon b)
    {
        return PolygonToPolygon(ref m, a, b);
    }
    #endregion
    #region Polygon
    public static bool PolygonToCircle(ref Manifold m, Polygon a, Circle b)
    {
        //Just switching the input so that we can pass it to the function above
        bool ret = CircleToPolygon(ref m, b, a);
        //Make sure that normal always points from A to B
        m.normal = -m.normal;
        return ret;
    }
    public static bool PolygonToCapsule(ref Manifold m, Polygon a, Capsule b)
    {
        //Just switching the input so that we can pass it to the function above
        bool ret = CapsuleToPolygon(ref m, b, a);
        //Make sure that normal always points from A to B
        m.normal = -m.normal;
        return ret;
    }
    public static bool PolygonToBox(ref Manifold m, Polygon a, Box b)
    {
        return PolygonToPolygon(ref m, a, b);
    }

    public static bool PolygonToPolygon(ref Manifold m, Polygon polyA, Polygon polyB)
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
        if (cp == 0)
            return false;
        else if (cp == 1)
            m.Update(contacts[0]);
        else
            m.Update(contacts[0], contacts[1]);

        m.contactCount = cp;
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

    #region Compound
    /// <summary>
    /// Tests a compound shape's children against another (non-compound) shape,
    /// keeping the child result with the greatest total penetration.
    /// </summary>
    /// <param name="isCompoundA">
    /// True if the compound is playing the role of "a" in the outer Collide(a, b)
    /// call — controls which side the child shape is plugged into so that
    /// m.normal still points from the outer "a" to the outer "b".
    /// </param>
    public static bool CompoundToOther(ref Manifold m, CompoundShape compound, Shape other, bool isCompoundA)
    {
        m.contactCount = 0;

        if (other.type == ShapeType.Compound)
            return CompoundToCompound(ref m, compound, (CompoundShape)other, isCompoundA);

        bool anyCollision = false;
        float deepestPenetration = float.MinValue;
        Manifold best = null;

        AABB otherAabb = other.GetBoundingBox();

        for (int i = 0; i < compound.Children.Count; i++)
        {
            ChildShape child = compound.Children[i];

            // Cheap reject before touching the expensive narrow-phase test.
            if (!GetChildWorldAABB(compound, child).Overlaps(in otherAabb))
                continue;

            // Scratch manifold reusing the real bodies so warm-start impulses
            // can still carry over once we commit the winning child below.
            Manifold childManifold = new Manifold(m.A, m.B);
            bool collided = TestChildVsShape(ref childManifold, compound, in child, other, isCompoundA);

            if (!collided)
                continue;

            if (best == null)
            {
                best = childManifold;
                anyCollision = true;
                continue;
            }

            // We bastardize our end-contacts so that multiple shapes can interact
            // with a single larger object, and we maximize contact count and depth.

            // First contact
            int childContacts = childManifold.contactCount;
            if (childContacts >= 1)
            {
                Contact contact = childManifold.contacts[0];

                if (best.contactCount < 2)
                {
                    best.contacts[1] = contact;
                    best.contactCount = 2;
                }
                else
                {
                    int smaller = best.contacts[0].penetration <= best.contacts[1].penetration ? 0 : 1;

                    if (contact.penetration > best.contacts[smaller].penetration)
                        best.contacts[smaller] = contact;
                }
            }

            // Second contact
            if (childContacts == 2)
            {
                Contact contact = childManifold.contacts[1];

                if (best.contactCount < 2)
                {
                    best.contacts[1] = contact;
                    best.contactCount = 2;
                }
                else
                {
                    int smaller = best.contacts[0].penetration <= best.contacts[1].penetration ? 0 : 1;

                    if (contact.penetration > best.contacts[smaller].penetration)
                        best.contacts[smaller] = contact;
                }
            }
            /*
            float totalPenetration = 0f;
            for (int c = 0; c < childManifold.contactCount; c++)
                totalPenetration += childManifold.contacts[c].penetration;

            if (totalPenetration > deepestPenetration)
            {
                deepestPenetration = totalPenetration;
                best = childManifold;
                anyCollision = true;
            }*/
        }

        if (anyCollision)
        {
            m.normal = best.normal;
            if (best.contactCount == 1)
                m.Update(best.contacts[0]);
            else if (best.contactCount == 2)
                m.Update(best.contacts[0], best.contacts[1]);
        }

        return anyCollision;
    }

    /// <summary>
    /// Tests a single compound child against a non-compound shape.
    /// Polygon/Box/Capsule children already have their local offset baked into
    /// their own vertex/endpoint data and share the compound's live Transform,
    /// so the ordinary Collide() dispatch poses them correctly with no extra
    /// work. Circle children are the one exception: a circle has no vertices
    /// to bake an offset into, so its (shared) Transform.Position always
    /// resolves to the compound's own center. We compute its true world
    /// position on the fly instead and go through the raw-position overloads.
    /// </summary>
    private static bool TestChildVsShape(ref Manifold m, CompoundShape compound, in ChildShape child, Shape other, bool childIsA)
    {
        if (child.Shape.type == ShapeType.Circle)
        {
            Circle circle = (Circle)child.Shape;
            Vector2 worldPos = compound.Transform.LocalToWorldPosition(child.LocalOffset);
            float radius = circle.radius * Lib.MathV.Max(circle.Transform.Scale);

            bool hit;
            switch (other.type)
            {
                case ShapeType.Circle:
                    Circle otherCircle = (Circle)other;
                    float otherRadius = otherCircle.radius * Lib.MathV.Max(otherCircle.Transform.Scale);
                    return childIsA
                        ? CircleToCircle(ref m, worldPos, radius, otherCircle.Transform.Position, otherRadius)
                        : CircleToCircle(ref m, otherCircle.Transform.Position, otherRadius, worldPos, radius);

                case ShapeType.Capsule:
                    hit = CircleToCapsule(ref m, worldPos, radius, (Capsule)other);
                    if (hit && !childIsA)
                        m.normal = -m.normal; // raw helper gives circle->other; flip back to other->circle
                    return hit;

                case ShapeType.Box:
                case ShapeType.Polygon:
                    hit = CircleToPolygon(ref m, worldPos, radius, (Polygon)other);
                    if (hit && !childIsA)
                        m.normal = -m.normal;
                    return hit;

                default:
                    return false;
            }
        }

        // Non-circle child: already correctly posed, just dispatch normally.
        return childIsA ? Collide(child.Shape, other, m) : Collide(other, child.Shape, m);
    }

    /// <summary>
    /// Tests every child of compoundA against every child of compoundB,
    /// keeping the deepest-penetrating pair.
    /// </summary>
    private static bool CompoundToCompound(ref Manifold m, CompoundShape compoundA, CompoundShape compoundB, bool isCompoundA)
    {
        m.contactCount = 0;

        bool anyCollision = false;
        float deepestPenetration = float.MinValue;
        Manifold best = null;

        // Both sides get re-checked once per opposing child in the loop below
        // (compoundA.Count * compoundB.Count times total), so compute each
        // child's AABB exactly once up front rather than recomputing it on
        // every inner-loop iteration.
        AABB[] aabbsA = new AABB[compoundA.Children.Count];
        for (int i = 0; i < compoundA.Children.Count; i++)
            aabbsA[i] = GetChildWorldAABB(compoundA, compoundA.Children[i]);

        AABB[] aabbsB = new AABB[compoundB.Children.Count];
        for (int j = 0; j < compoundB.Children.Count; j++)
            aabbsB[j] = GetChildWorldAABB(compoundB, compoundB.Children[j]);

        for (int i = 0; i < compoundA.Children.Count; i++)
        {
            ChildShape childA = compoundA.Children[i];

            for (int j = 0; j < compoundB.Children.Count; j++)
            {
                // Cheap reject before touching the expensive narrow-phase test.
                if (!aabbsA[i].Overlaps(aabbsB[j]))
                    continue;

                ChildShape childB = compoundB.Children[j];

                Manifold childManifold = new Manifold(m.A, m.B);
                bool collided = TestChildVsChild(ref childManifold, compoundA, in childA, compoundB, in childB);

                if (!collided)
                    continue;


                if (best == null)
                {
                    best = childManifold;
                    anyCollision = true;
                    continue;
                }

                // We bastardize our end-contacts so that multiple shapes can interact
                // with a single larger object, and we maximize contact count and depth.

                // First contact
                int childContacts = childManifold.contactCount;
                if (childContacts >= 1)
                {
                    Contact contact = childManifold.contacts[0];

                    if (best.contactCount < 2)
                    {
                        best.contacts[1] = contact;
                        best.contactCount = 2;
                    }
                    else
                    {
                        int smaller = best.contacts[0].penetration <= best.contacts[1].penetration ? 0 : 1;

                        if (contact.penetration > best.contacts[smaller].penetration)
                            best.contacts[smaller] = contact;
                    }
                }

                // Second contact
                if (childContacts == 2)
                {
                    Contact contact = childManifold.contacts[1];

                    if (best.contactCount < 2)
                    {
                        best.contacts[1] = contact;
                        best.contactCount = 2;
                    }
                    else
                    {
                        int smaller = best.contacts[0].penetration <= best.contacts[1].penetration ? 0 : 1;

                        if (contact.penetration > best.contacts[smaller].penetration)
                            best.contacts[smaller] = contact;
                    }
                }

                /*
                float totalPenetration = 0f;
                for (int c = 0; c < childManifold.contactCount; c++)
                    totalPenetration += childManifold.contacts[c].penetration;

                if (totalPenetration > deepestPenetration)
                {
                    deepestPenetration = totalPenetration;
                    best = childManifold;
                    anyCollision = true;
                }
                */
            }
        }

        if (anyCollision)
        {
            // TestChildVsChild always resolves normal childA->childB. If this
            // compound wasn't the outer "a", flip so m.normal stays a -> b.
            m.normal = isCompoundA ? best.normal : -best.normal;
            if (best.contactCount == 1)
                m.Update(best.contacts[0]);
            else if (best.contactCount == 2)
                m.Update(best.contacts[0], best.contacts[1]);
        }

        return anyCollision;
    }

    /// <summary>
    /// Tests one child of compoundA against one child of compoundB. Result
    /// normal always points from childA toward childB regardless of which
    /// compound was the outer "a" — CompoundToCompound flips it if needed.
    /// </summary>
    private static bool TestChildVsChild(ref Manifold m, CompoundShape compoundA, in ChildShape childA, CompoundShape compoundB, in ChildShape childB)
    {
        bool aIsCircle = childA.Shape.type == ShapeType.Circle;
        bool bIsCircle = childB.Shape.type == ShapeType.Circle;

        if (aIsCircle && bIsCircle)
        {
            Circle circleA = (Circle)childA.Shape;
            Circle circleB = (Circle)childB.Shape;
            Vector2 posA = compoundA.Transform.LocalToWorldPosition(childA.LocalOffset);
            Vector2 posB = compoundB.Transform.LocalToWorldPosition(childB.LocalOffset);
            float radA = circleA.radius * Lib.MathV.Max(circleA.Transform.Scale);
            float radB = circleB.radius * Lib.MathV.Max(circleB.Transform.Scale);
            return CircleToCircle(ref m, posA, radA, posB, radB);
        }

        if (aIsCircle)
        {
            Circle circleA = (Circle)childA.Shape;
            Vector2 posA = compoundA.Transform.LocalToWorldPosition(childA.LocalOffset);
            float radA = circleA.radius * Lib.MathV.Max(circleA.Transform.Scale);

            return childB.Shape.type switch
            {
                ShapeType.Capsule => CircleToCapsule(ref m, posA, radA, (Capsule)childB.Shape),
                ShapeType.Box or ShapeType.Polygon => CircleToPolygon(ref m, posA, radA, (Polygon)childB.Shape),
                _ => false
            };
        }

        if (bIsCircle)
        {
            Circle circleB = (Circle)childB.Shape;
            Vector2 posB = compoundB.Transform.LocalToWorldPosition(childB.LocalOffset);
            float radB = circleB.radius * Lib.MathV.Max(circleB.Transform.Scale);

            bool hit = childA.Shape.type switch
            {
                ShapeType.Capsule => CircleToCapsule(ref m, posB, radB, (Capsule)childA.Shape),
                ShapeType.Box or ShapeType.Polygon => CircleToPolygon(ref m, posB, radB, (Polygon)childA.Shape),
                _ => false
            };
            if (hit)
                m.normal = -m.normal; // raw helper gives circleB->childA; flip to childA->childB
            return hit;
        }

        // Neither child is a circle - both already correctly posed via their
        // own compound's shared Transform + baked-in local offsets.
        return Collide(childA.Shape, childB.Shape, m);
    }

    /// <summary>
    /// World-space AABB for a single compound child. Polygon/Box/Capsule children
    /// can just use their own GetBoundingBox() (correct, since their vertex/
    /// endpoint data already has the local offset baked in and they share the
    /// compound's live Transform). Circle children need the same live
    /// world-position fix as collision testing, since their shared
    /// Transform.Position resolves to the compound's center, not their own.
    /// </summary>
    private static AABB GetChildWorldAABB(CompoundShape compound, in ChildShape child)
    {
        if (child.Shape.type == ShapeType.Circle)
        {
            Circle circle = (Circle)child.Shape;
            Vector2 worldPos = compound.Transform.LocalToWorldPosition(child.LocalOffset);
            float radius = circle.radius * Lib.MathV.Max(circle.Transform.Scale);
            Vector2 extent = new Vector2(radius, radius);
            return new AABB(worldPos - extent, worldPos + extent);
        }

        return child.Shape.GetBoundingBox();
    }
    #endregion

}
using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Dynamics.Shapes;

namespace Rubedo.Physics2D.Math;

/// <summary>
/// Applies the internal-edge fix to contacts generated against a compound's
/// polygon children: if the reference face used for the contact is a welded
/// seam, and the neighboring face's normal disagrees enough to risk catching
/// a body on the seam, blend toward the neighbor's normal instead.
/// </summary>
public static class InternalEdgeUtility
{
    // Face normals within this dot product are already "smooth enough" -
    // nothing to correct.
    private const float SAME_SURFACE_DOT_THRESHOLD = 0.999f;

    // How close (world units) a contact must be to the shared vertex before
    // we consider it a seam-corner case rather than a normal face hit.
    private const float SEAM_PROXIMITY = 0.05f;

    public static void TryCorrectNormal(CompoundShape compound, int childIndex, Manifold m)
    {
        if (compound.Children[childIndex].Shape is not Polygon poly)
            return;

        int edgeIndex = FindMatchingEdge(poly, m.normal);
        if (edgeIndex < 0)
            return;

        EdgeAdjacency adj = compound.GetEdgeAdjacency(childIndex, edgeIndex);
        if (!adj.IsInternal)
            return;

        if (compound.Children[adj.NeighborChildIndex].Shape is not Polygon neighborPoly)
            return;

        Vector2 neighborNormal = neighborPoly.transformedNormals[adj.NeighborEdgeIndex];

        float dot = Vector2.Dot(m.normal, neighborNormal);
        if (dot > SAME_SURFACE_DOT_THRESHOLD)
            return; // already smooth, no correction needed

        // Only treat this as a seam-corner case if a contact point is
        // actually near the shared vertex - otherwise this is a legitimate
        // hit against the face interior and must not be touched.
        Vector2 sharedVertex = poly.transformedVertices[(edgeIndex + 1) % poly.VertexCount];
        bool nearSeam = false;
        for (int i = 0; i < m.contactCount; i++)
        {
            if (Vector2.DistanceSquared(m.contacts[i].position, sharedVertex) < SEAM_PROXIMITY * SEAM_PROXIMITY)
            {
                nearSeam = true;
                break;
            }
        }
        if (!nearSeam)
            return;

        // Blend rather than fully snap, and never rotate the normal past
        // perpendicular to either face - that would let the body punch
        // through instead of sliding smoothly.
        Vector2 corrected = m.normal + neighborNormal;
        if (Lib.Math.NearlyEqual(corrected.LengthSquared(), 0))
            return; // faces point opposite ways - real corner, leave it alone

        MathV.Normalize(ref corrected);
        m.normal = corrected;
    }

    private static int FindMatchingEdge(Polygon poly, Vector2 normal)
    {
        int best = -1;
        float bestDot = 0.99f; // require a close match to the actual reference face
        for (int i = 0; i < poly.VertexCount; i++)
        {
            float dot = Vector2.Dot(poly.transformedNormals[i], normal);
            if (dot > bestDot)
            {
                bestDot = dot;
                best = i;
            }
        }
        return best;
    }
}

using Microsoft.Xna.Framework;
using Rubedo.Object;
using Rubedo.Physics2D.Common;
using Rubedo.Physics2D.Dynamics.Shapes;
using Rubedo.Physics2D.Math;
using System.Collections.Generic;

namespace Rubedo.Physics2D.Dynamics;
public class CompoundBody : PhysicsBody
{
    internal override Transform TargetTransform {
        get
        {
            return _targetTransform;
        }
        set
        {
            _targetTransform = value;
            foreach (ChildShape child in Shape.Children)
            {
                child.Shape.SetTransform(value);
            }
        }
    }
    private Transform _targetTransform;

    public CompoundShape Shape => (CompoundShape)base.collider.shape;

    /// <summary>
    /// Gets the number of child shapes in this compound body.
    /// </summary>
    public int ChildCount => Shape.Children.Count;

    public CompoundBody(Collider collider, PhysicsMaterial material) : base(collider, material)
    {
        if (collider.shape.type != ShapeType.Compound)
            throw new System.ArgumentException("Trying to build compound body out of non-compound type!", nameof(collider));

        RecalculateMassProperties();
    }

    /// <summary>
    /// Construct a compound shape collider from a series of polygons. Polygons are assumed to be in the same transformation space as the compound.
    /// </summary>
    /// <param name="polygons"></param>
    /// <returns></returns>
    public static Collider FromPolygons(List<Polygon> polygons, bool isTrigger = false)
    {
        CompoundShape compound = new CompoundShape();

        //Vector2 compoundCentroid = ComputeOverallCentroid(polygons);

        for (int i = 0; i < polygons.Count; i++)
        {
            Polygon polygon = polygons[i];

            Vector2 centroid = ShapeUtility.ComputeCentroid(polygon.vertices);
            Vector2 localOffset = centroid;

            float mass = polygon.GetArea();
            compound.AddChild(polygon, Vector2.Zero, 0, mass);
        }
        Collider collider = new Collider(compound, isTrigger);
        return collider;
    }

    public static Collider FromVertexData(Vector2[] vertices, bool isTrigger = false)
    {
        //TODO: Implement polygon decomposition to construct polygons.
        throw new System.NotImplementedException();
    }

    private static Vector2 ComputeOverallCentroid(List<Polygon> polygons)
    {
        Vector2 weightedSum = Vector2.Zero;
        float totalArea = 0f;

        foreach (Polygon polygon in polygons)
        {
            var centroid = ShapeUtility.ComputeCentroid(polygon.vertices);
            float area = polygon.GetArea();
            weightedSum += centroid * area;
            totalArea += area;
        }

        return totalArea > 0 ? weightedSum / totalArea : Vector2.Zero;
    }

    /// <summary>
    /// Recalculates mass and inertia from child shapes.
    /// </summary>
    private void RecalculateMassProperties()
    {
        float totalMass = 0f;
        float totalInertia = 0f;

        foreach (var child in Shape.Children)
        {
            totalMass += child.Mass;
            // Parallel axis theorem: I = I_cm + m*d^2
            totalInertia += child.Inertia + child.Mass * Vector2.Dot(child.LocalOffset, child.LocalOffset);
        }

        // Use protected setters from base class
        _mass = totalMass;
        _invMass = totalMass > 0 ? 1f / totalMass : 0f;
        _inertia = totalInertia;
        _invInertia = totalInertia > 0 ? 1f / totalInertia : 0f;
    }
}

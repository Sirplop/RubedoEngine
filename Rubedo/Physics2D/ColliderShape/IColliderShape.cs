using Microsoft.Xna.Framework;
using Rubedo.Object;

namespace Rubedo.Physics2D.ColliderShape;

public interface IColliderShape
{

    // Shape Enum so we don't have to run IsInstanceOfClass 100 million times every minute
    public ShapeType ShapeType { get; }
    public ref AABB Bounds { get; }
    public AABB RegisteredBounds { get; set; }

    public bool TransformUpdateRequired { get; set; }
    public bool BoundsUpdateRequired { get; set; }

    /// <summary>
    /// Returns the area of the shape.
    /// </summary>
    float GetArea();
    /// <summary>
    /// Computes the axis-aligned bounding box for this shape and its transform.
    /// </summary>
    public void RecalculateAABB();
    
    /// <summary>
    /// Returns the moment of inertia for the shape given a mass.
    /// </summary>
    float GetMomentOfInertia(float mass);
    public void TransformVertices();
}

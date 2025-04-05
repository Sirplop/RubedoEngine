using Rubedo.Object;

namespace Rubedo.Physics2D.Collision.Shapes;
public interface IShape
{
    public ShapeType ShapeType { get; }
    public ref AABB Bounds { get; }
    public bool BoundsUpdateRequired { get; set; }


    /// <summary>
    /// Returns the localized area of the shape.
    /// </summary>
    float GetArea();
    /// <summary>
    /// Computes the axis-aligned bounding box for this shape and its transform.
    /// </summary>
    void RecalculateAABB();
    /// <summary>
    /// Returns the moment of inertia for the shape given a mass.
    /// </summary>
    float GetMomentOfInertia(float mass);
}

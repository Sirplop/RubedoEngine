using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D.Math;

namespace Rubedo.Physics2D.Dynamics.Shapes;

public abstract class Shape
{
    public Transform transform => _transform;
    internal Transform _transform;
    internal AABB RegisteredBounds; //used for broadphase

    internal bool transformDirty;
    internal bool normalsDirty;
    internal bool boundsDirty;

    public Shape(Transform refTransform)
    {
        _transform = refTransform;
    }

    public ShapeType type { get; protected set; }

    public abstract AABB GetBoundingBox();
    public abstract float GetArea();
    public abstract float GetMomentOfInertia(float mass);
    public abstract bool Raycast(Ray2D ray, float distance, out RaycastResult result);
}

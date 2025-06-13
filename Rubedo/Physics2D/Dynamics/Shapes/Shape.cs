using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D.Math;

namespace Rubedo.Physics2D.Dynamics.Shapes;

public abstract class Shape
{
    public Transform Transform => _transform;
    protected Transform _transform;
    internal AABB RegisteredBounds; //used for broadphase

    internal bool transformDirty;
    internal bool normalsDirty;
    internal bool boundsDirty;

    internal void SetTransform(Transform transform)
    {
        _transform = transform;
        transformDirty = true;
        normalsDirty = true;
        boundsDirty = true;
    }

    public ShapeType type { get; protected set; }

    public abstract AABB GetBoundingBox();
    public abstract float GetArea();
    public abstract float GetMomentOfInertia(float mass);
    public abstract bool Raycast(Ray2D ray, float distance, out RaycastResult result);
}

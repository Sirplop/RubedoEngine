using PhysicsEngine2D;
using Rubedo.Object;
using Rubedo.Physics2D.Collision.Shapes;

namespace Rubedo.Physics2D.Dynamics.Shapes;

public abstract class Shape
{
    public Transform transform => _transform;
    protected Transform _transform;
    internal AABB RegisteredBounds; //used for broadphase

    public Shape(Transform refTransform)
    {
        _transform = refTransform;
    }

    public ShapeType type { get; protected set; }

    public abstract AABB GetBoundingBox();
    public abstract float GetArea();
    public abstract float GetMomentOfInertia(float mass);
    public abstract bool Raycast(Ray2 ray, float distance, out RaycastResult result);
}

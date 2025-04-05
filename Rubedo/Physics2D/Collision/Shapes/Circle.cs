using Microsoft.Xna.Framework;
using Rubedo.Object;

namespace Rubedo.Physics2D.Collision.Shapes;

/// <summary>
/// I am Circle, and this is my summary.
/// </summary>
public class Circle : IShape
{
    public ShapeType ShapeType => ShapeType.Circle;
    public ref AABB Bounds
    {
        get
        {
            if (_boundsUpdateRequired)
                RecalculateAABB();
            return ref _bounds;
        }
    }
    protected AABB _bounds;
    public bool BoundsUpdateRequired { get => _boundsUpdateRequired; set => _boundsUpdateRequired = value; }
    private bool _boundsUpdateRequired = true;

    internal Transform transform;
    internal float radius;

    public Circle(Transform transform, float radius)
    {
        this.transform = transform;
        this.radius = radius;
        _bounds = new AABB();
    }

    public float GetArea()
    {
        return MathHelper.Pi * radius * radius;
    }

    public float GetMomentOfInertia(float mass)
    {
        return 0.5f * mass * radius * radius;
    }

    public void RecalculateAABB()
    {
        // A circle's AABB is independent of rotation.
        Vector2 pos = transform.WorldPosition;
        float scale = Lib.Math.Max(transform.WorldScale);
        _bounds.Set(new Vector2((pos.X - radius) * scale, (pos.Y - radius) * scale),
            new Vector2((pos.X + radius) * scale, (pos.Y + radius) * scale));
        _boundsUpdateRequired = false;
    }
}
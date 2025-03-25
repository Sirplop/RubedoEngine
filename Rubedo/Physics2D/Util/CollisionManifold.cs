
using Microsoft.Xna.Framework;

namespace Rubedo.Physics2D.Util;

/// <summary>
/// I am CollisionManifold, and this is my summary.
/// </summary>
public record CollisionManifold
{
    public PhysicsObject bodyA;
    public PhysicsObject bodyB;
    public float depth;
    public Vector2 normal;
    public Vector2 contactPoint1;
    public Vector2? contactPoint2;

    public void Reset()
    {
        bodyA = null;
        bodyB = null;
        depth = 0;
        normal = Vector2.Zero;
        contactPoint1 = Vector2.Zero;
        contactPoint2 = null;
    }
}
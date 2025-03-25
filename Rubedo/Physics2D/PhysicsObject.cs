namespace Rubedo.Physics2D;

/// <summary>
/// A simple data type holding both a physics body and a collider.
/// </summary>
public record PhysicsObject
{
    public PhysicsBody body;
    public Collider collider;

    public PhysicsObject(PhysicsBody body, Collider collider)
    {
        this.body = body;
        this.collider = collider;
    }
}
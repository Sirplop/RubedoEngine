namespace Rubedo.Physics2D.Common;

/// <summary>
/// Contains information about the properties of physics objects.
/// </summary>
public class PhysicsMaterial
{
    public float density;
    public float restitution;
    public float linearDamping;
    public float angularDamping;

    public float friction;

    public PhysicsMaterial(float density, float friction, float restitution, float linearDamping = 0, float angularDamping = 0)
    {
        this.density = density;
        this.friction = friction;
        this.restitution = restitution;
        this.linearDamping = linearDamping;
        this.angularDamping = angularDamping;
    }
}
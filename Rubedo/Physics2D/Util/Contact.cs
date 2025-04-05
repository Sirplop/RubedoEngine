using Microsoft.Xna.Framework;

namespace Rubedo.Physics2D.Util;

/// <summary>
/// Stores information about a contact point in a physics collision.
/// </summary>
internal sealed class Contact
{
    public Vector2 position;
    public float penetration;

    public float accumImpulse;
    public float accumFriction;
    public float normalMass;
    public float tangentMass;

    public float velocityBias;
    public float positionBias;

    internal Vector2 ra;
    internal Vector2 rb;

    public Contact(Vector2 position, float penetration = 0)
    {
        this.position = position;
        this.penetration = penetration;
    }

    public void Reset()
    {
        position = Vector2.Zero;
        penetration = 0;
        accumImpulse = 0;
        accumFriction = 0; //everything else is set when needed.
    }
}
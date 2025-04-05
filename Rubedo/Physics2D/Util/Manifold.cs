using Microsoft.Xna.Framework;
using Rubedo.Physics2D.Dynamics;
using System;

namespace Rubedo.Physics2D.Util;

/// <summary>
/// Stores information about a physics collision and its contact points.
/// </summary>
internal sealed record Manifold : IEquatable<Manifold>
{
    public PhysicsBody A;
    public PhysicsBody B;
    public Vector2 normal;

    public Contact[] contacts = new Contact[2] { new Contact(Vector2.Zero), new Contact(Vector2.Zero) };
    public int contactCount;
    internal float friction;

    public void Reset()
    {
        A = null;
        B = null;
        normal = Vector2.Zero;
        contactCount = 0;
        contacts[0].Reset();
        contacts[1].Reset();
    }

    public Contact GetContact(int i)
    {
        return contacts[i];
    }
    public bool Equals(Manifold other)
    {
        return other.A.Equals(A) && other.B.Equals(B) || other.B.Equals(A) && other.A.Equals(B);
    }

    public override int GetHashCode()
    {
        return A.GetHashCode() + B.GetHashCode();
    }
}
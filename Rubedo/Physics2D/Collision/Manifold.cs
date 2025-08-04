using Microsoft.Xna.Framework;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Math;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rubedo.Physics2D.Collision;

public class Contact
{
    public Vector2 Position => position;
    public float Penetration => penetration;

    internal Vector2 position;
    internal float accumImpulse;
    internal float accumFriction;

    //Bias based on penetration of bodies
    internal float penetration;

    internal float bias;

    internal float normalMass;
    internal float tangentMass;

    internal Vector2 ra;
    internal Vector2 rb;

    public Contact(Vector2 position, float impulse = 0, float friction = 0, float penetration = 0)
    {
        this.position = position;
        accumFriction = friction;
        accumImpulse = impulse;
        this.penetration = penetration;
    }

    public Contact Clone()
    {
        return new Contact(position, accumImpulse, accumFriction, penetration);
    }
}

internal enum ManifoldState { New, Stay, Exit }
public class Manifold : IEquatable<Manifold>
{
    public Vector2 Normal => normal;
    public Vector2 Tangent => tangent;
    public ReadOnlyCollection<Contact> ContactPoints => contacts.AsReadOnly();
    public int ContactCount => contactCount;

    public readonly PhysicsBody A;
    public readonly PhysicsBody B;
    internal Vector2 normal;
    internal Vector2 tangent;

    //We only need two contact points
    internal Contact[] contacts = new Contact[2];
    internal int contactCount;

    internal float friction;
    internal float restitution;
    internal bool noImpulse;
    internal ManifoldState state;

    public Manifold(PhysicsBody bodyA, PhysicsBody bodyB)
    {
        A = bodyA;
        B = bodyB;
        normal = default;
        noImpulse = bodyA.collider.isTrigger || bodyB.collider.isTrigger;
        state = ManifoldState.New;
    }

    public void Update(Contact c)
    {
        Contact cOld = contacts[0];

        if (cOld != null)
        {
            c.accumFriction = cOld.accumFriction;
            c.accumImpulse = cOld.accumImpulse;
        }
        contacts[0] = c;

        contactCount = 1;
    }

    public void Update(Contact c1, Contact c2)
    {
        Contact cOld = contacts[0];

        if (cOld != null)
        {
            c1.accumFriction = cOld.accumFriction;
            c1.accumImpulse = cOld.accumImpulse;
        }
        contacts[0] = c1;

        cOld = contacts[1];
        if (cOld != null)
        {
            c2.accumFriction = cOld.accumFriction;
            c2.accumImpulse = cOld.accumImpulse;
        }
        contacts[1] = c2;

        contactCount = 2;
    }

    internal bool SolveContact()
    {
        //Check whether colliding and fill data in us
        return PhysicsCollisions.Collide(A.collider.shape, B.collider.shape, this);
    }

    //Required for Broad Phase
    public bool Equals(Manifold other)
    {
        return other.A.Equals(A) && other.B.Equals(B) || other.A.Equals(B) && other.B.Equals(A);
    }

    //Required for Broad Phase
    public override int GetHashCode()
    {
        return A.GetHashCode() + B.GetHashCode();
    }
}
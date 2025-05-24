using Microsoft.Xna.Framework;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Math;
using System;
using System.Collections.Generic;

namespace Rubedo.Physics2D.Collision;

public class Contact
{
    public Vector2 position;
    public float accumImpulse;
    public float accumFriction;

    //Bias based on penetration of bodies
    public float penetration;

    public float bias;

    public float normalMass;
    public float tangentMass;

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

public class Manifold : IEquatable<Manifold>
{
    public readonly PhysicsBody A;
    public readonly PhysicsBody B;
    internal Vector2 normal;
    internal Vector2 tangent;

    //We only need two contact points
    internal Contact[] contacts = new Contact[2];
    internal int contactCount;

    internal float friction;
    internal float restitution;

    public Manifold(PhysicsBody bodyA, PhysicsBody bodyB)
    {
        A = bodyA;
        B = bodyB;
        normal = default;
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

    [Obsolete]
    public void Update(int numNewContacts, params Contact[] newContacts)
    {
        Contact[] mergedContacts = new Contact[2];

        for (int i = 0; i < numNewContacts; i++)
        {
            Contact cOld = contacts[i];
            Contact cNew = newContacts[i];
            mergedContacts[i] = cNew.Clone();

            if (cOld != null)
            {
                mergedContacts[i].accumFriction = cOld.accumFriction;
                mergedContacts[i].accumImpulse = cOld.accumImpulse;
            }
        }

        for (int i = 0; i < numNewContacts; ++i)
            contacts[i] = mergedContacts[i].Clone();

        contactCount = numNewContacts;
    }

    public bool SolveContact()
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
using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Util;
using System;

namespace Rubedo.Physics2D.Constraints;

/// <summary>
/// Solves physics collisions using PGS + Baumgarte stabilization.
/// </summary>
internal static class ContactSolver
{
    public static void PresolveConstraint(in Manifold m, float invDT)
    {
        const float PENETRATION_SLOP = 0.01f;
        const float BAUMGARTE = 0.2f;

        PhysicsBody bodyA = m.A;
        PhysicsBody bodyB = m.B;

        float inverseMassSum = bodyA.invMass + bodyB.invMass;
        float e = MathF.Min(bodyA.material.restitution, bodyB.material.restitution);
        m.friction = (bodyA.material.friction + bodyB.material.friction) * 0.5f;
        Vector2 tangent = Lib.Math.Right(m.normal, 1);

        for (int i = 0; i < m.contactCount; i++)
        {
            Contact c = m.contacts[i];

            // Relative velocity at contact
            c.ra = c.position - bodyA.transform.WorldPosition;
            c.rb = c.position - bodyB.transform.WorldPosition;

            Vector2 rv = (bodyB.LinearVelocity + Lib.Math.Left(c.rb, bodyB.AngularVelocity)) -
                         (bodyA.LinearVelocity + Lib.Math.Left(c.ra, bodyA.AngularVelocity));

            // Restitution * normal velocity at first impact
            float vn = Vector2.Dot(rv, m.normal);

            // Restitution bias
            c.velocityBias = 0;
            if (vn < -1)
            {
                c.velocityBias = e * vn;
            }

            // Effective masses

            /*
                Effective mass

                1   1   (r⊥ᴬᴾ · n)^2  (r⊥ᴮᴾ · n)^2
                ─ + ─ + ─────────── + ───────────
                Mᴬ  Mᴮ      Iᴬ            Iᴮ
            */

            float rn1 = Lib.Math.Cross(c.ra, m.normal);
            float rn2 = Lib.Math.Cross(c.rb, m.normal);

            c.normalMass = inverseMassSum + bodyA.invInertia * (rn1 * rn1) +
                                            bodyB.invInertia * (rn2 * rn2);
            c.normalMass = 1 / c.normalMass;

            float rt1 = Lib.Math.Cross(c.ra, tangent);
            float rt2 = Lib.Math.Cross(c.rb, tangent);

            c.tangentMass = inverseMassSum + bodyA.invInertia * (rt1 * rt1) +
                                             bodyB.invInertia * (rt2 * rt2);
            c.tangentMass = 1 / c.tangentMass;

            // Position error is fed back to the velocity constraint as a bias value
            float correction = MathF.Min(-c.penetration + PENETRATION_SLOP, 0.0f);
            c.positionBias = BAUMGARTE * invDT * correction;

            // Perfect restitution + baumgarte leads to overshooting
            if (c.velocityBias < c.positionBias)
                c.velocityBias -= c.positionBias;
        }
    }

    public static void WarmStart(in Manifold m)
    {
        PhysicsBody bodyA = m.A;
        PhysicsBody bodyB = m.B;
        Vector2 tangent = Lib.Math.Right(m.normal);

        for (int i = 0; i < m.contactCount; i++)
        {
            Contact c = m.contacts[i];
            if (c.penetration < 0f) continue;

            //Accumulated impulses
            Vector2 p = c.accumImpulse * m.normal + c.accumFriction * tangent;

            bodyA.ApplyImpulse(-p, c.ra);
            bodyB.ApplyImpulse(p, c.rb);
        }
    }

    public static void ApplyImpulse(in Manifold m)
    {
        PhysicsBody bodyA = m.A;
        PhysicsBody bodyB = m.B;

        if (bodyA.invMass + bodyB.invMass == 0) return;

        Vector2 tangent = Lib.Math.Right(m.normal, 1);

        /*
            In an iterative solver what is applied the last affects the result more.
            So we solve normal impulse after tangential impulse because
            non-penetration is more important.
        */
        //solve friction

        if (m.friction == 0) 
            goto normal; //don't solve friction if there isn't any.

        for (int i = 0; i < m.contactCount; i++)
        {
            Contact c = m.contacts[i];

            // Relative velocity at contact
            Vector2 rv = (bodyB.LinearVelocity + Lib.Math.Left(c.rb, bodyB.AngularVelocity)) -
                         (bodyA.LinearVelocity + Lib.Math.Left(c.ra, bodyA.AngularVelocity));

            // Tangential impulse magnitude
            float lambda = -Vector2.Dot(rv, tangent) * c.tangentMass;

            //accumulate tangential impulse
            float maxPt = c.accumImpulse * m.friction;
            float pt0 = c.accumFriction;
            c.accumFriction = Lib.Math.Clamp(pt0 + lambda, -maxPt, maxPt);
            lambda = c.accumFriction - pt0;

            Vector2 impulse = tangent * lambda;

            bodyA.ApplyImpulse(-impulse, c.ra);
            bodyB.ApplyImpulse(impulse, c.rb);
        }

        //solve normal
        normal:  for (int i = 0; i < m.contactCount; i++)
        {
            Contact c = m.contacts[i];

            // Relative velocity at contact
            Vector2 rv = (bodyB.LinearVelocity + Lib.Math.Left(c.rb, bodyB.AngularVelocity)) -
                         (bodyA.LinearVelocity + Lib.Math.Left(c.ra, bodyA.AngularVelocity));

            float vn = Vector2.Dot(rv, m.normal);

            //Calculate impulse scalar
            float j = -(vn + c.velocityBias + c.positionBias);
            j *= c.normalMass;

            //Find impulse to apply after clamping since we applied the accumulated impulse in the warm start
            float pn0 = c.accumImpulse;
            c.accumImpulse = MathF.Max(pn0 + j, 0);
            j = c.accumImpulse - pn0;

            Vector2 pn = j * m.normal;
            bodyA.ApplyImpulse(-pn, c.ra);
            bodyB.ApplyImpulse(pn, c.rb);
        }
    }
}
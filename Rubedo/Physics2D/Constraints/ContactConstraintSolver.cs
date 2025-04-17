using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Physics2D.Collision;
using System;

namespace Rubedo.Physics2D.Constraints;

/// <summary>
/// I am ContactConstraintSolver, and this is my summary.
/// </summary>
public static class ContactConstraintSolver
{
    private static float lambda;
    private static Vector2 rv;
    public static void PresolveConstraint(Manifold m, float invDT)
    {
        const float PENETRATION_SLOP = 0.01f;
        const float BAUMGARTE = 0.2f;

        float invMassSum = m.A.invMass + m.B.invMass;
        float e = MathF.Min(m.A.material.restitution, m.B.material.restitution);
        m.friction = (m.A.material.friction + m.B.material.friction) * 0.5f;
        MathV.Right(ref m.normal, out m.tangent);

        for (int i = 0; i < m.contactCount; i++)
        {
            Contact c = m.contacts[i];

            // Relative velocity at contact
            c.ra = c.position - m.A.position;
            c.rb = c.position - m.B.position;

            RelVel(ref m, ref c, out rv);

            // Restitution * normal velocity at first impact
            Vector2.Dot(ref rv, ref m.normal, out float vn);

            // Restitution bias
            float velocityBias = 0;
            if (vn < -1)
            {
                velocityBias = e * vn;
            }

            // Effective masses

            /*
                Effective mass

                1   1   (r⊥ᴬᴾ · n)^2  (r⊥ᴮᴾ · n)^2
                ─ + ─ + ─────────── + ───────────
                Mᴬ  Mᴮ      Iᴬ            Iᴮ
            */

            MathV.Cross(ref c.ra, ref m.normal, out float rn1);
            MathV.Cross(ref c.rb, ref m.normal, out float rn2);

            c.normalMass = 1f / (invMassSum + m.A.invInertia * (rn1 * rn1) +
                                        m.B.invInertia * (rn2 * rn2));

            MathV.Cross(ref c.ra, ref m.tangent, out float rt1);
            MathV.Cross(ref c.rb, ref m.tangent, out float rt2);

            c.tangentMass = 1f / (invMassSum + m.A.invInertia * (rt1 * rt1) +
                                         m.B.invInertia * (rt2 * rt2));

            // Position error is fed back to the velocity constraint as a bias value
            float correction = MathF.Min(-c.penetration + PENETRATION_SLOP, 0.0f);//Math.Max(0.0f, c.penetration - PENETRATION_SLOP);
            c.bias = BAUMGARTE * invDT * correction;

            // Perfect restitution + baumgarte leads to overshooting
            if (velocityBias < c.bias)
                velocityBias -= c.bias;
            c.bias += velocityBias;
        }
    }

    public static void WarmStart(Manifold m)
    {
        for (int i = 0; i < m.contactCount; i++)
        {
            Contact c = m.contacts[i];
            if (c.penetration < 0f) continue;

            //Accumulated impulses
            MathV.MulAdd2(ref m.normal, c.accumImpulse, ref m.tangent, c.accumFriction, out Vector2 p);

            m.A.ApplyImpulse(ref p, ref c.ra, true);
            m.B.ApplyImpulse(ref p, ref c.rb, false);
        }
    }
    public static void ApplyImpulse(Manifold m)
    {
        if (m.A.invMass + m.B.invMass == 0) return;

        /*
            In an iterative solver what is applied the last affects the result more.
            So we solve normal impulse after tangential impulse because
            non-penetration is more important.
        */
        if (m.friction == 0)
            goto normal; //skip friction if there isn't any

        //solve friction
        for (int i = 0; i < m.contactCount; i++)
        {
            Contact c = m.contacts[i];

            // Relative velocity at contact
            RelVel(ref m, ref c, out rv);

            // Tangential impulse magnitude
            Vector2.Dot(ref rv, ref m.tangent, out lambda);
            lambda *= -c.tangentMass;

            //accumulate tangential impulse
            float maxPt = c.accumImpulse * m.friction;
            float pt0 = c.accumFriction;
            c.accumFriction = Lib.Math.Clamp(pt0 + lambda, -maxPt, maxPt);
            lambda = c.accumFriction - pt0;

            Vector2.Multiply(ref m.tangent, lambda, out rv);

            //apply impulses
            m.A.ApplyImpulse(ref rv, ref c.ra, true);
            m.B.ApplyImpulse(ref rv, ref c.rb, false);
        }

        //solve penetration
        normal: for (int i = 0; i < m.contactCount; i++)
        {
            Contact c = m.contacts[i];

            // Relative velocity at contact
            RelVel(ref m, ref c, out rv);

            Vector2.Dot(ref rv, ref m.normal, out lambda);

            //Calculate impulse scalar
            lambda = -(lambda + c.bias) * c.normalMass;

            //Find impulse to apply after clamping since we applied the accumulated impulse in the warm start
            float pn0 = c.accumImpulse;
            c.accumImpulse = MathF.Max(pn0 + lambda, 0);
            lambda = c.accumImpulse - pn0;

            Vector2.Multiply(ref m.normal, lambda, out rv);

            //apply impulses
            m.A.ApplyImpulse(ref rv, ref c.ra, true);
            m.B.ApplyImpulse(ref rv, ref c.rb, false);
        }
    }

    private static void RelVel(ref Manifold m, ref Contact c, out Vector2 rv)
    {
        rv.X = m.B.velocity.X - (c.rb.Y * m.B.angularVelocity) - (m.A.velocity.X - (c.ra.Y * m.A.angularVelocity));
        rv.Y = m.B.velocity.Y + (c.rb.X * m.B.angularVelocity) - (m.A.velocity.Y + (c.ra.X * m.A.angularVelocity));
    }
}
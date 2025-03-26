//#define PHYSICS_DEBUG

using Microsoft.Xna.Framework;
using Rubedo.Debug;
using Rubedo.Lib;
using Rubedo.Physics2D.Util;
using Rubedo.Render;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Rubedo.Physics2D;

/// <summary>
/// I am PhysicsWorld, and this is my summary.
/// </summary>
public class PhysicsWorld
{
    public const float FPS = 50;
    private const float DT = 1 / FPS;
    private const int PHYSICS_ITERATIONS = 8;
    private double accumulator = 0;

    public Vector2 gravity;

    protected List<PhysicsObject> bodies;
    protected List<Collider> triggerColliders; //TODO: Triggers.

    private readonly ManifoldPool _manifoldPool = new ManifoldPool();
    private readonly List<(int, int)> collisionPairs = new List<(int, int)>();

    //reusable values.
    private readonly SpatialHashGrid spatialHashGrid;
    private readonly HashSet<(PhysicsObject, PhysicsObject)> _pairSet =
        new HashSet<(PhysicsObject, PhysicsObject)>(new ColliderPairComparer());

    public int BodyCount => bodies.Count;

    private double BroadPhaseTime = 0;
    private double NarrowPhaseTime = 0;

#if PHYSICS_DEBUG
    //debug drawing
    public readonly List<Vector2> contactPoints = new List<Vector2>();
#endif

    /// <summary>
    /// Constructs a new physics system.
    /// </summary>
    public PhysicsWorld()
    {
        gravity = new Vector2(0f, -9.81f * RubedoEngine.SizeOfMeter);
        bodies = new List<PhysicsObject>();
        spatialHashGrid = new SpatialHashGrid(40);
    }

    public void AddBody(PhysicsObject body)
    {
        if (bodies.Contains(body))
            throw new System.Exception("World already contains body!");
        bodies.Add(body);
        spatialHashGrid.Add(body);
    }
    public bool RemoveBody(PhysicsObject body)
    {
        bool ret = bodies.Remove(body);
        if (ret)
            spatialHashGrid.Remove(body);
        return ret;
    }

    public bool GetBody(int i, out PhysicsObject body)
    {
        body = null;
        if (i < 0 || i >= bodies.Count)
            return false;
        body = bodies[i];
        return true;
    }

    public void Update()
    {
        //UpdatePhysics(RubedoEngine.DeltaTime);
        //accumulator += RubedoEngine.DeltaTime;

        // Avoid accumulator spiral of death by clamping
        if (accumulator > 0.1f)
            accumulator = 0.1f;

        //while (accumulator > DT)
        {
            //BroadphasePairing();
            UpdatePhysics(RubedoEngine.DeltaTime);
            //accumulator -= DT;
        }
    }
    private void UpdatePhysics(float deltaTime)
    {
#if PHYSICS_DEBUG
        contactPoints.Clear();
#endif
        float iter = deltaTime / PHYSICS_ITERATIONS;
        double broadTime = 0;
        double narrowTime = 0;
        for (int it = 0; it < PHYSICS_ITERATIONS; it++)
        {
            // Movement step
            for (int i = 0; i < bodies.Count; i++)
            {
                this.bodies[i].body.Step(iter);
            }

            RubedoEngine.Instance.physicsPhaseWatch.Restart();
            Broadphase();
            RubedoEngine.Instance.physicsPhaseWatch.Stop();
            broadTime += RubedoEngine.Instance.physicsPhaseWatch.Elapsed.TotalMilliseconds;
            RubedoEngine.Instance.physicsPhaseWatch.Restart();
            Narrowphase();
            RubedoEngine.Instance.physicsPhaseWatch.Stop();
            narrowTime += RubedoEngine.Instance.physicsPhaseWatch.Elapsed.TotalMilliseconds;
        }

        BroadPhaseTime += (broadTime - BroadPhaseTime) * 0.1f;
        NarrowPhaseTime += (narrowTime - NarrowPhaseTime) * 0.1f;
        RubedoEngine.debugText.DrawText(new Vector2(0, 35), $"Broad Phase time: {BroadPhaseTime.ToString("0.00")} | Narrow Phase time: {NarrowPhaseTime.ToString("0.00")}", true);
    }
    #region Broad Phase
    private void Broadphase()
    {
        for (int i = 0; i < this.bodies.Count - 1; i++)
        {
            PhysicsObject bodyA = this.bodies[i];

            for (int j = i + 1; j < this.bodies.Count; j++)
            {
                PhysicsObject bodyB = this.bodies[j];

                if (bodyA.body.isStatic && bodyB.body.isStatic)
                    continue;

                if (!bodyA.collider.shape.Bounds.Intersects(ref bodyB.collider.shape.Bounds))
                    continue;
                collisionPairs.Add((i, j));
            }
        }
    }
    #endregion
    #region Narrow Phase
    public void Narrowphase()
    {
        for (int i = 0; i < collisionPairs.Count; i++)
        {
            PhysicsObject A = bodies[collisionPairs[i].Item1]; 
            PhysicsObject B = bodies[collisionPairs[i].Item2];
            CollisionManifold m = _manifoldPool.Get();
            if (PhysicsCollisions.CheckCollide(A.collider.shape, B.collider.shape, ref m))
            {
                SeparateBodies(A.body, B.body, m.normal * m.depth);
                m.bodyA = A;
                m.bodyB = B;
                this.ResolveCollision(m);

#if PHYSICS_DEBUG
                contactPoints.Add(m.contactPoint1);
                if (m.contactPoint2.HasValue)
                    contactPoints.Add(m.contactPoint2.Value);
#endif
                _manifoldPool.Release(m);
            }
            else
                _manifoldPool.Release(m); //no collision, so return manifold to pool.
        }
        collisionPairs.Clear();
    }
    #endregion

    public void SeparateBodies(in PhysicsBody bodyA, in PhysicsBody bodyB, Vector2 minTranslation)
    {
        if (bodyA.isStatic)
        {
            bodyB.Move(minTranslation);
        }
        else if (bodyB.isStatic)
        {
            bodyA.Move(-minTranslation);
        }
        else
        {
            bodyA.Move(-minTranslation * 0.5f);
            bodyB.Move(minTranslation * 0.5f);
        }
    }

    private Vector2[] contactList = new Vector2[2];
    private Vector2[] impulseList = new Vector2[2];
    private Vector2[] raList = new Vector2[2];
    private Vector2[] rbList = new Vector2[2];
    private float[] jValue = new float[2];

    public void ResolveCollision(in CollisionManifold contact)
    {
        PhysicsBody bodyA = contact.bodyA.body;
        PhysicsBody bodyB = contact.bodyB.body;
        Vector2 normal = contact.normal;
        Vector2 contact1 = contact.contactPoint1;
        Vector2 contact2 = contact.contactPoint2.HasValue ? contact.contactPoint2.Value : Vector2.Zero;
        int contactCount = contact.contactPoint2.HasValue ? 2 : 1;

        float staticFriction = (bodyA.staticFriction + bodyB.staticFriction) * 0.5f;
        float dynamicFriction = (bodyA.dynamicFriction + bodyB.dynamicFriction) * 0.5f;

        float e = MathF.Min(bodyA.restitution, bodyB.restitution);

        this.contactList[0] = contact1;
        this.contactList[1] = contact2;

        for (int i = 0; i < 2; i++)
        {
            this.impulseList[i] = Vector2.Zero;
            this.raList[i] = Vector2.Zero;
            this.rbList[i] = Vector2.Zero;
            jValue[i] = 0;
        }

        //Resolve linear and angular impulses
        for (int i = 0; i < contactCount; i++)
        {
            Vector2 ra = contactList[i] - bodyA.worldTransform.Position;
            Vector2 rb = contactList[i] - bodyB.worldTransform.Position;

            raList[i] = ra;
            rbList[i] = rb;

            Vector2 raPerp = new Vector2(-ra.Y, ra.X);
            Vector2 rbPerp = new Vector2(-rb.Y, rb.X);

            Vector2 angularLinearVelocityA = raPerp * bodyA.AngularVelocity;
            Vector2 angularLinearVelocityB = rbPerp * bodyB.AngularVelocity;

            Vector2 relativeVelocity =
                (bodyB.LinearVelocity + angularLinearVelocityB) -
                (bodyA.LinearVelocity + angularLinearVelocityA);

            float contactVelocityMag = Vector2.Dot(relativeVelocity, normal);

            if (contactVelocityMag > 0f)
            {
                continue;
            }

            float raPerpDotN = Vector2.Dot(raPerp, normal);
            float rbPerpDotN = Vector2.Dot(rbPerp, normal);

            float denom = bodyA.invMass + bodyB.invMass +
                (raPerpDotN * raPerpDotN) * bodyA.invInertia +
                (rbPerpDotN * rbPerpDotN) * bodyB.invInertia;

            float j = -(1f + e) * contactVelocityMag;
            j /= denom;
            j /= (float)contactCount;

            jValue[i] = j;
            impulseList[i] = j * normal;
        }

        for (int i = 0; i < contactCount; i++)
        {
            Vector2 impulse = impulseList[i];
            Vector2 ra = raList[i];
            Vector2 rb = rbList[i];

            bodyA.LinearVelocity += -impulse * bodyA.invMass;
            bodyA.AngularVelocity += -Lib.Math.Cross(ra, impulse) * bodyA.invInertia;
            bodyB.LinearVelocity += impulse * bodyB.invMass;
            bodyB.AngularVelocity += Lib.Math.Cross(rb, impulse) * bodyB.invInertia;
        }

        //Resolve friction impulses
        for (int i = 0; i < contactCount; i++)
        {
            Vector2 ra = contactList[i] - bodyA.worldTransform.Position;
            Vector2 rb = contactList[i] - bodyB.worldTransform.Position;

            raList[i] = ra;
            rbList[i] = rb;

            Vector2 raPerp = new Vector2(-ra.Y, ra.X);
            Vector2 rbPerp = new Vector2(-rb.Y, rb.X);

            Vector2 angularLinearVelocityA = raPerp * bodyA.AngularVelocity;
            Vector2 angularLinearVelocityB = rbPerp * bodyB.AngularVelocity;

            Vector2 relativeVelocity =
                (bodyB.LinearVelocity + angularLinearVelocityB) -
                (bodyA.LinearVelocity + angularLinearVelocityA);

            Vector2 tangent = relativeVelocity - Vector2.Dot(relativeVelocity, contact.normal) * contact.normal;

            if (Lib.Math.NearlyEqual(tangent, Vector2.Zero))
            {
                impulseList[i] = Vector2.Zero;
                continue;
            }
            else
                tangent = Vector2Ext.Normalize(tangent);

            float raPerpDotT = Vector2.Dot(raPerp, tangent);
            float rbPerpDotT = Vector2.Dot(rbPerp, tangent);

            float denom = bodyA.invMass + bodyB.invMass +
                (raPerpDotT * raPerpDotT) * bodyA.invInertia +
                (rbPerpDotT * rbPerpDotT) * bodyB.invInertia;

            float jt = -Vector2.Dot(relativeVelocity, tangent);
            jt /= denom;
            jt /= (float)contactCount;
            Vector2 frictionImpulse;
            float j = jValue[i];
            //coulomb's law
            if (MathF.Abs(jt) <= j * staticFriction)
                frictionImpulse = jt * tangent;
            else
                frictionImpulse = -j * tangent * dynamicFriction;

            impulseList[i] = frictionImpulse;
        }

        for (int i = 0; i < contactCount; i++)
        {
            Vector2 frictionImpulse = impulseList[i];
            Vector2 ra = raList[i];
            Vector2 rb = rbList[i];

            bodyA.LinearVelocity += -frictionImpulse * bodyA.invMass;
            bodyA.AngularVelocity += -Lib.Math.Cross(ra, frictionImpulse) * bodyA.invInertia;
            bodyB.LinearVelocity += frictionImpulse * bodyB.invMass;
            bodyB.AngularVelocity += Lib.Math.Cross(rb, frictionImpulse) * bodyB.invInertia;
        }
    }


    public static void PositionalCorrection(PhysicsBody bodyA, PhysicsBody bodyB, ref float depth, ref Vector2 normal)
    {
        var percent = 0.6f; // usually 20% to 80%
        var slop = 0.05f;    // usually 0.01 to 0.1

        // Only correct penetration beyond the slop.
        float penetration = MathF.Max(depth - slop, 0.0f);
        float correctionMagnitude = penetration / (bodyA.mass + bodyB.mass) * percent;
        Vector2 correction = normal * correctionMagnitude;

        if (!bodyA.isStatic)
        {
            bodyA.Move(-correction * bodyA.mass);
        }

        if (!bodyB.isStatic)
        {
            bodyB.Move(correction * bodyB.mass);
        }
    }

    /// <summary>
    /// Custom comparer for collision pair tuples so that the order of objects does not matter.
    /// </summary>
    private class ColliderPairComparer : IEqualityComparer<(PhysicsObject, PhysicsObject)>
    {
        public bool Equals((PhysicsObject, PhysicsObject) x, (PhysicsObject, PhysicsObject) y)
        {
            return (ReferenceEquals(x.Item1, y.Item1) && ReferenceEquals(x.Item2, y.Item2)) ||
                   (ReferenceEquals(x.Item1, y.Item2) && ReferenceEquals(x.Item2, y.Item1));
        }

        public int GetHashCode((PhysicsObject, PhysicsObject) pair)
        {
            // XOR is order-independent.
            return pair.Item1.GetHashCode() ^ pair.Item2.GetHashCode();
        }
    }

    public void DebugDraw(Shapes shapes)
    {
        spatialHashGrid.DebugDraw(shapes);
    }
}
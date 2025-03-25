using Microsoft.Xna.Framework;
using Rubedo.Physics2D.Util;
using Rubedo.Render;
using System;
using System.Collections.Generic;

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
    private readonly List<CollisionPair> collisionPairs = new List<CollisionPair>();

    //reusable values.
    private readonly SpatialHashGrid spatialHashGrid;
    private readonly HashSet<(PhysicsObject, PhysicsObject)> _pairSet =
        new HashSet<(PhysicsObject, PhysicsObject)>(new ColliderPairComparer());
    private readonly List<CollisionManifold> contacts = new List<CollisionManifold>();

    public int BodyCount => bodies.Count;

    //debug drawing
    public readonly List<Vector2> contactPoints = new List<Vector2>();

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
#if FALSE
    private void UpdatePhysics(float deltaTime)
    {
        float iter = deltaTime / PHYSICS_ITERATIONS;
        for (int x = 0; x < PHYSICS_ITERATIONS; x++)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                bodies[i].body?.Step(iter);
            }
            for (int i = 0; i < collisionPairs.Count; i++)
            {
                CollisionPair pair = collisionPairs[i];
                PhysicsObject bodyA = pair.A;
                PhysicsObject bodyB = pair.B;

                //we don't have to check if the bodies are static because they will never both be static.

                if (PhysicsCollisions.CheckCollide(bodyA.collider.shape, bodyB.collider.shape, out float depth, out Vector2 normal))
                {
                    if (bodyA.body.isStatic)
                    {
                        bodyB.body.Move(normal * depth);
                    }
                    else if (bodyB.body.isStatic)
                    {
                        bodyA.body.Move(-normal * depth);
                    }
                    else
                    {
                        bodyA.body.Move(-normal * depth * 0.5f);
                        bodyB.body.Move(normal * depth * 0.5f);
                    }

                    CollisionManifold m = _manifoldPool.Get();
                    m.bodyA = bodyA;
                    m.bodyB = bodyB;
                    m.depth = depth;
                    m.normal = normal;
                    contacts.Add(m);
                }
            }
            for (int i = 0; i < contacts.Count; i++)
            {
                this.ResolveCollision(contacts[i]);
                _manifoldPool.Release(contacts[i]);
            }
            contacts.Clear();
        }
    }
#elif TRUE
    private void UpdatePhysics(float deltaTime)
    {
        contactPoints.Clear();
        float iter = deltaTime / PHYSICS_ITERATIONS;
        for (int it = 0; it < PHYSICS_ITERATIONS; it++)
        {
            // Movement step
            for (int i = 0; i < bodies.Count; i++)
            {
                this.bodies[i].body.Step(iter);
            }

            // collision step
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

                    CollisionManifold m = _manifoldPool.Get();
                    if (PhysicsCollisions.CheckCollide(bodyA.collider.shape, bodyB.collider.shape, ref m))
                    {
                        if (bodyA.body.isStatic)
                        {
                            bodyB.body.Move(m.normal * m.depth);
                        }
                        else if (bodyB.body.isStatic)
                        {
                            bodyA.body.Move(-m.normal * m.depth);
                        }
                        else
                        {
                            bodyA.body.Move(-m.normal * m.depth * 0.5f);
                            bodyB.body.Move(m.normal * m.depth * 0.5f);
                        }
                        m.bodyA = bodyA;
                        m.bodyB = bodyB;
                        contacts.Add(m);
                    }
                    else
                        _manifoldPool.Release(m); //no collision, so return manifold to pool.
                }
            }
            for (int i = 0; i < contacts.Count; i++)
            {
                this.ResolveCollision(contacts[i]);
                _manifoldPool.Release(contacts[i]);
            }
            contacts.Clear();
        }
    }
#endif
    #region Broadphase
    private void BroadphasePairing()
    {
        collisionPairs.Clear();
        _pairSet.Clear();

        for (int i = 0; i < bodies.Count; i++)
        {
            if (bodies[i].collider.TransformChanged())
                spatialHashGrid.Update(bodies[i]);
        }

        for (int i = 0; i < bodies.Count; i++)
        {
            if (bodies[i].collider == null || bodies[i].body == null || bodies[i].body.isStatic)
                continue; //static bodies do not themselves register collisions.
            spatialHashGrid.AABB_Broadphase(bodies[i], _pairSet, collisionPairs);
        }
    }
    #endregion

    public void ResolveCollision(in CollisionManifold manifold)
    {
        contactPoints.Add(manifold.contactPoint1);
        if (manifold.contactPoint2.HasValue)
            contactPoints.Add(manifold.contactPoint2.Value);

        PhysicsBody bodyA = manifold.bodyA.body;
        PhysicsBody bodyB = manifold.bodyB.body;

        Vector2 relativeVelocity = bodyB.LinearVelocity - bodyA.LinearVelocity;

        //we're already traveling in the appropriate direction.
        if (Vector2.Dot(relativeVelocity, manifold.normal) > 0f)
            return;

        float e = MathF.Min(bodyA.restitution, bodyB.restitution);

        float j = -(1f + e) * Vector2.Dot(relativeVelocity, manifold.normal);
        j /= bodyA.invMass + bodyB.invMass;

        Vector2 impulse = j * manifold.normal;

        bodyA.LinearVelocity -= impulse * bodyA.invMass;
        bodyB.LinearVelocity += impulse * bodyB.invMass;
    }

    public static void PositionalCorrection(PhysicsBody bodyA, PhysicsBody bodyB, ref float depth, ref Vector2 normal)
    {
        var percent = 0.6f; // usually 20% to 80%
        var slop = 0.05f;    // usually 0.01 to 0.1

        // Only correct penetration beyond the slop.
        float penetration = Math.Max(depth - slop, 0.0f);
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
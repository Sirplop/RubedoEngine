//#define USE_RANDY
#define USE_EVIL
//#define USE_SOLVER2D

using Microsoft.Xna.Framework;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Constraints;
using Rubedo.Physics2D.Dynamics;
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
    public const int PHYSICS_ITERATIONS = 7;

    public Vector2 gravity;
    public void ResetGravity() =>  gravity = new Vector2(0f, -9.81f * RubedoEngine.SizeOfMeter);

    internal float Damping { get; private set; }

    protected List<PhysicsBody> bodies;

    private readonly ManifoldPool _manifoldPool = new ManifoldPool();
    private readonly HashSet<Manifold> collisionPairs = new HashSet<Manifold>();

    public int BodyCount => bodies.Count;

    private double BroadPhaseTime = 0;
    private double NarrowPhaseTime = 0;

    /// <summary>
    /// Constructs a new physics system.
    /// </summary>
    public PhysicsWorld()
    {
        gravity = new Vector2(0f, -9.81f * RubedoEngine.SizeOfMeter);
        bodies = new List<PhysicsBody>();
        Damping = 0.99f;
    }

    public void AddBody(PhysicsBody body)
    {
        if (bodies.Contains(body))
            throw new System.Exception("World already contains body!");
        bodies.Add(body);
    }
    public bool RemoveBody(PhysicsBody body)
    {
        bool ret = bodies.Remove(body);
        return ret;
    }

    public bool GetBody(int i, out PhysicsBody body)
    {
        body = null;
        if (i < 0 || i >= bodies.Count)
            return false;
        body = bodies[i];
        return true;
    }

    public void Step()
    {
        if (bodies.Count == 0) 
            return; //nothing to update, so skip it.

        int i = 0;
        float deltaTime = RubedoEngine.DeltaTime;
        float invDT = 1f / deltaTime;

        //compute pairs (broad phase)
        RubedoEngine.Instance.physicsPhaseWatch.Restart();

        for (; i < bodies.Count; i++)
            bodies[i].IntegrateForces(deltaTime);

        Broadphase();

        RubedoEngine.Instance.physicsPhaseWatch.Stop();
        double broadTime = RubedoEngine.Instance.physicsPhaseWatch.Elapsed.TotalMilliseconds;

        RubedoEngine.Instance.physicsPhaseWatch.Restart();
        
        //narrow phase
        SolveContacts();

        foreach (Manifold m in collisionPairs)
            ContactSolver.PresolveConstraint(m, invDT);

        foreach (Manifold m in collisionPairs)
            ContactSolver.WarmStart(m);

        //Process maxIterations times for more stability
        for (i = 0; i < PHYSICS_ITERATIONS; i++)
            foreach (Manifold m in collisionPairs)
                ContactSolver.ApplyImpulse(m);

        //Integrate positions
        for (i = 0; i < bodies.Count; i++)
            bodies[i].IntegrateVelocities(deltaTime);

        RubedoEngine.Instance.physicsPhaseWatch.Stop();
        double narrowTime = RubedoEngine.Instance.physicsPhaseWatch.Elapsed.TotalMilliseconds;

        BroadPhaseTime += (broadTime - BroadPhaseTime) * 0.1f;
        NarrowPhaseTime += (narrowTime - NarrowPhaseTime) * 0.1f;
        RubedoEngine.debugText.DrawText(new Vector2(0, 35), $"Broad Phase time: {BroadPhaseTime.ToString("00.00")} | Narrow Phase time: {NarrowPhaseTime.ToString("00.00")}", true);
    }
    #region Broad Phase
    private readonly HashSet<Manifold> copy =   new HashSet<Manifold>();
    private void Broadphase()
    {
        for (int i = 0; i < this.bodies.Count - 1; i++)
        {
            PhysicsBody bodyA = this.bodies[i];

            for (int j = i + 1; j < this.bodies.Count; j++)
            {
                PhysicsBody bodyB = this.bodies[j];

                if (bodyA.isStatic && bodyB.isStatic)
                    continue;

                if (!bodyA.collider.shape.Bounds.Intersects(ref bodyB.collider.shape.Bounds))
                    continue;

                Manifold m = _manifoldPool.Get();
                m.A = bodyA;
                m.B = bodyB;
                if (!collisionPairs.Contains(m))
                {
                    collisionPairs.Add(m);
                    copy.Add(m);
                }
                else
                    _manifoldPool.Release(m);
            }
        }
        collisionPairs.RemoveWhere(m => !copy.Contains(m));
        copy.Clear();
    }

    private void FreeManifolds()
    {
        foreach (Manifold m in collisionPairs)
        {
            _manifoldPool.Release(m);
        }
        collisionPairs.Clear();
    }
    #endregion
    #region Narrow Phase
    public void SolveContacts()
    {
        foreach (Manifold m in collisionPairs)
        {
            Manifold man = m;
            if (!PhysicsCollisions.Collide(m.A.collider.shape, m.B.collider.shape, ref man))
            {

            }
        }
    }
   
    #endregion

    public void DebugDraw(Shapes shapes)
    {
        /*foreach (CollisionManifold m in collisionPairs)
        {
            for (int i = 0; i < m.contactCount; i++)
            {
                Contact c = m.contacts[i];
                Vector2 lineEnd = c.position + (m.normal * c.depth);
                shapes.DrawBox(c.position, 5, 5, 0, Vector2.One, Color.Red);
                shapes.DrawLine(c.position, lineEnd, Color.Magenta);
            }
        }*/
    }
}
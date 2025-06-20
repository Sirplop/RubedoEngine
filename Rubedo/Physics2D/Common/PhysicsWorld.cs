using Microsoft.Xna.Framework;
using Rubedo.EngineDebug;
using Rubedo.Graphics;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Collision.Broadphase;
using Rubedo.Physics2D.Constraints;
using Rubedo.Physics2D.Dynamics;
using System.Collections.Generic;

namespace Rubedo.Physics2D.Common;

public class PhysicsWorld
{
    public static Vector2 gravity = new Vector2(0, -9.81f * RubedoEngine.SizeOfMeter);
    public static void ResetGravity() => gravity = new Vector2(0f, -9.81f * RubedoEngine.SizeOfMeter);

    public static bool bruteForce = false;
    public static bool showContacts = false;
    public static bool drawBroadphase = false;

    public float fixedDeltaTime = 0.02f;

    public List<PhysicsBody> bodies = new List<PhysicsBody>();

    internal HashSet<Manifold> manifoldSet = new HashSet<Manifold>();
    internal List<Manifold> manifolds = new List<Manifold>();

    private IBroadphase broadphase;
    private double accumulatedDelta = 0;

    public Timer timer;

    public int ManifoldCount
    {
        get
        {
            return manifolds.Count;
        }
    }

    private int maxIterations = 8;

    public PhysicsWorld()
    {
        // Switch the collision system here:
        broadphase = new SpatialHashGrid(2);
        timer = new Timer();
    }

    public void AddBody(PhysicsBody b)
    {
        bodies.Add(b);
        broadphase.Add(b);
    }

    public void RemoveBody(PhysicsBody b)
    {
        bodies.Remove(b);
        broadphase.Remove(b);
    }

    public void Clear()
    {
        bodies.Clear();
        manifolds.Clear();
        manifoldSet.Clear();
        broadphase.Clear();
    }

    public bool Raycast(Vector2 origin, Vector2 direction, out RaycastResult result)
    {
        return Raycast(origin, direction, Math.Ray2D.TMAX, out result);
    }

    public bool Raycast(Vector2 origin, Vector2 direction, float distance, out RaycastResult result)
    {
        return Raycast(new Math.Ray2D(origin, direction), distance, out result);
    }

    public bool Raycast(Math.Ray2D ray, out RaycastResult result)
    {
        return Raycast(ray, Math.Ray2D.TMAX, out result);
    }

    public bool Raycast(Math.Ray2D ray, float distance, out RaycastResult result)
    {
        return broadphase.Raycast(ray, distance, out result);
    }

    public void Tick(float dt)
    {
        accumulatedDelta += dt;

        // Avoid accumulator death spiral
        if (accumulatedDelta > 0.1f)
            accumulatedDelta = 0.1f;

        while (accumulatedDelta > fixedDeltaTime)
        {
            Update(fixedDeltaTime);
            accumulatedDelta -= fixedDeltaTime;
        }
    }

    private void Update(float dt)
    {
        if (bodies.Count == 0)
            return; //nothing to update.

        timer.Reset();
        int i;

        for (i = 0; i < bodies.Count; i++)
            bodies[i].Update();

        for (i = 0; i < bodies.Count; i++)
            bodies[i].IntegrateForces(dt);

        timer.Start();
        if (bruteForce)
        {
            for (i = 0; i < bodies.Count - 1; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    if (bodies[i].isStatic && bodies[j].isStatic)
                        continue;
                    if (bodies[i].bounds.Overlaps(bodies[j].bounds))
                    {
                        Manifold key = new Manifold(bodies[i], bodies[j]);
                        if (manifoldSet.Add(key))
                        {
                            manifolds.Add(key);
                        }
                    }
                }
            }
            timer.Step("B.BF: ");
        }
        else
        {
            //Broad phase
            broadphase.Update(bodies);
            timer.Step("B.U: ");
            broadphase.ComputePairs(manifolds, manifoldSet);
            timer.Step("B.C: ");
        }

        //Narrow phase
        SolveContacts();
        timer.Step("N.SC: ");

        float invDT = 1f / dt;
        for (i = 0; i < manifolds.Count; i++)
            ContactConstraintSolver.PresolveConstraint(manifolds[i], invDT);
        timer.Step("N.PC: ");

        for (i = 0; i < manifolds.Count; i++)
            ContactConstraintSolver.WarmStart(manifolds[i]);
        timer.Step("N.WS: ");

        //Process maxIterations times for more stability
        for (int j = 0; j < maxIterations; j++)
            for (i = 0; i < manifolds.Count; i++)
                ContactConstraintSolver.ApplyImpulse(manifolds[i]);

        timer.Step("N.AI: ");
        //Integrate positions
        for (i = 0; i < bodies.Count; i++)
            bodies[i].IntegrateVelocity(dt);
        timer.Stop("N.IV: ");
    }

    private void SolveContacts()
    {
        for (int i = manifolds.Count - 1; i >= 0; i--)
        {
            Manifold m = manifolds[i];
            if (!m.SolveContact())
            {
                manifolds[i] = manifolds[manifolds.Count - 1];
                manifolds.RemoveAt(manifolds.Count - 1); //swap and remove to remove unecessary moves
                manifoldSet.Remove(m);
            }
        }
    }

    public void DebugDraw(Shapes shapes)
    {
        if (drawBroadphase)
            broadphase.DebugDraw(shapes);

        if (showContacts)
        {
            float contactSize = RubedoEngine.SizeOfMeter * 0.1f;
            foreach (Manifold m in RubedoEngine.Instance.World.manifolds)
            {
                for (int i = 0; i < m.contactCount; i++)
                {
                    Contact c = m.contacts[i];
                    Vector2 lineEnd = c.position + m.normal * c.penetration;
                    shapes.DrawBox(c.position, contactSize, contactSize, 0, Vector2.One, Color.Red);
                    shapes.DrawLine(c.position, lineEnd, Color.Magenta);
                }
            }
        }
    }
}

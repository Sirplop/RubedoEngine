using Microsoft.Xna.Framework;
using Rubedo;
using Rubedo.Debug;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Collision.Broadphase;
using Rubedo.Physics2D.Constraints;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Render;
using System.Collections.Generic;

namespace PhysicsEngine2D;

public class PhysicsWorld
{
    public static Vector2 gravity = new Vector2(0, -9.81f * RubedoEngine.SizeOfMeter);
    public void ResetGravity() => gravity = new Vector2(0f, -9.81f * RubedoEngine.SizeOfMeter);

    public static bool bruteForce = false;
    public static bool showContacts = false;
    public static bool drawBroadphase = false;

    public List<PhysicsBody> bodies = new List<PhysicsBody>();

    internal HashSet<Manifold> manifolds = new HashSet<Manifold>();

    private IBroadphase broadphase;

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
        broadphase.Clear();
    }

    public bool Raycast(Vector2 origin, Vector2 direction, out RaycastResult result)
    {
        return Raycast(origin, direction, Ray2.Tmax, out result);
    }

    public bool Raycast(Vector2 origin, Vector2 direction, float distance, out RaycastResult result)
    {
        return Raycast(new Ray2(origin, direction), distance, out result);
    }

    public bool Raycast(Ray2 ray, out RaycastResult result)
    {
        return Raycast(ray, Ray2.Tmax, out result);
    }

    public bool Raycast(Ray2 ray, float distance, out RaycastResult result)
    {
        return broadphase.Raycast(ray, distance, out result);
    }

    public void Update(float dt)
    {
        Timer timer = new Timer();
        foreach (PhysicsBody b in bodies)
            b.Update();

        foreach (PhysicsBody b in bodies)
            b.IntegrateForces(dt);

        timer.Start();
        if (bruteForce)
        {
            for (int i = 0; i < bodies.Count - 1; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    if (bodies[i].isStatic && bodies[j].isStatic)
                        continue;
                    if (bodies[i].bounds.Overlaps(bodies[j].bounds))
                    {
                        Manifold key = new Manifold(bodies[i], bodies[j]);
                        if (!manifolds.Contains(key))
                            manifolds.Add(key);
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
            broadphase.ComputePairs(manifolds);
            timer.Step("B.C: ");
        }

        //Narrow phase
        SolveContacts();
        timer.Step("N.SC: ");

        float invDT = 1f / dt;
        foreach (Manifold m in manifolds)
            ContactConstraintSolver.PresolveConstraint(m, invDT);
        timer.Step("N.PC: ");
        foreach (Manifold m in manifolds)
            ContactConstraintSolver.WarmStart(m);
        timer.Step("N.WS: ");
        //Process maxIterations times for more stability
        for (int i = 0; i < maxIterations; i++)
            foreach (Manifold m in manifolds)
                ContactConstraintSolver.ApplyImpulse(m);

        timer.Step("N.AI: ");
        //Integrate positions
        foreach (PhysicsBody b in bodies)
            b.IntegrateVelocity(dt);
        timer.Stop("N.IV: ");

        DebugText.Instance.DrawTextStack(timer.GetAsString(", "));
    }

    private void SolveContacts()
    {
        foreach (Manifold m in manifolds)
        {
            if (m.A.Entity == null || m.B.Entity == null || !m.SolveContact())
            {
                manifolds.Remove(m);
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
                    Vector2 lineEnd = c.position + (m.normal * c.penetration);
                    shapes.DrawBox(c.position, contactSize, contactSize, 0, Vector2.One, Color.Red);
                    shapes.DrawLine(c.position, lineEnd, Color.Magenta);
                }
            }
        }
        Vector2 mouse = RubedoEngine.Input.MouseWorldPosition();
        RubedoEngine.Instance.Camera.GetExtents(out Vector2 min, out _);
        shapes.DrawLine(mouse, new Vector2(mouse.X, min.Y), Color.DarkRed);
    }
}

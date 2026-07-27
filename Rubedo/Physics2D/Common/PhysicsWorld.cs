using Microsoft.Xna.Framework;
using Rubedo.EngineDebug;
using Rubedo.Graphics;
using Rubedo.Lib.Extensions;
using Rubedo.Lib.Threading;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Collision.Broadphase;
using Rubedo.Physics2D.Constraints;
using Rubedo.Physics2D.Dynamics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rubedo.Physics2D.Common;

public class PhysicsWorld
{
    private FixedThreadDispatcher dispatcher = new FixedThreadDispatcher();

    public static Vector2 gravity = new Vector2(0, -9.81f * RubedoEngine.SizeOfMeter);
    public static void ResetGravity() => gravity = new Vector2(0f, -9.81f * RubedoEngine.SizeOfMeter);

    public static bool multithreadSolver = false;
    public static bool showContacts = false;
    public static bool drawBroadphase = false;

    public List<PhysicsBody> bodies = new List<PhysicsBody>();

    internal List<Manifold> manifolds = new List<Manifold>();

    // --- Island / sleep bookkeeping ---
    private int[] islandParent = new int[0];
    private readonly Dictionary<int, float> islandMinSleepTime = new Dictionary<int, float>();
    private readonly Dictionary<int, bool> islandCanSleep = new Dictionary<int, bool>();

    private IBroadphase broadphase;

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
        broadphase = new SpatialHashGrid(3);
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

    internal void Step(float dt)
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

        //Broad phase
        broadphase.Update(bodies);
        timer.Step("B.U: ");
        broadphase.ComputePairs(manifolds);
        timer.Step("B.C: ");

        //Narrow phase
        SolveContacts();
        timer.Step("N.SC: ");

        UpdateIslandsAndSleep(dt);
        timer.Step("N.ISLAND: ");

        float invDT = 1f / dt;
        if (multithreadSolver)
        {
            dispatcher.For(0, manifolds.Count, i =>
            {
                Manifold m = manifolds[i];
                if (!m.noImpulse && (IsActive(m.A) || IsActive(m.B)))
                    ContactConstraintSolver.PresolveConstraint(m, invDT);
            });
        }
        else
        {
            for (i = 0; i < manifolds.Count; i++)
            {
                Manifold m = manifolds[i];
                if (!m.noImpulse && (IsActive(m.A) || IsActive(m.B)))
                    ContactConstraintSolver.PresolveConstraint(m, invDT);
            }
        }
        timer.Step("N.PC: ");

        for (i = 0; i < manifolds.Count; i++)
        {
            Manifold m = manifolds[i];
            if (!m.noImpulse && (IsActive(m.A) || IsActive(m.B)))
                ContactConstraintSolver.WarmStart(m);
        }
        timer.Step("N.WS: ");

        //Process maxIterations times for more stability
        for (int j = 0; j < maxIterations; j++)
            for (i = 0; i < manifolds.Count; i++)
            {
                Manifold m = manifolds[i];
                if (!m.noImpulse && (IsActive(m.A) || IsActive(m.B)))
                    ContactConstraintSolver.ApplyImpulse(in m);
            }

        timer.Step("MC: "+ manifolds.Count + " N.AI: ");
        //Integrate positions
        for (i = 0; i < bodies.Count; i++)
            bodies[i].IntegrateVelocity(dt);
        timer.Stop("N.IV: ");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsActive(PhysicsBody b) => !b.isStatic && b.IsAwake;

    private bool[] contacts = new bool[128];
    private void SolveContacts()
    {
        if (multithreadSolver)
        {
            if (contacts.Length < manifolds.Count)
                contacts = new bool[Lib.Math.Power2Roundup(manifolds.Count)];

            dispatcher.For(0, manifolds.Count, (i) =>
            {
                Manifold m = manifolds[i];
                contacts[i] = m.SolveContact();
            });

            for (int i = manifolds.Count - 1; i >= 0; i--)
            {
                Manifold m = manifolds[i];
                if (!contacts[i])
                {
                    m.A.FinalizeCollisions(m);
                    manifolds.SwapAndRemove(i);
                    broadphase.RemoveManifold(in m);
                }
                else
                {
                    m.A.DoCollision(m);
                }
            }
        }
        else
        {

            for (int i = manifolds.Count - 1; i >= 0; i--)
            {
                Manifold m = manifolds[i];
                if (!m.SolveContact())
                {
                    m.A.FinalizeCollisions(m);
                    manifolds.SwapAndRemove(i); //swap and remove to remove unecessary moves
                    broadphase.RemoveManifold(in m);
                } else
                {
                    m.A.DoCollision(m);
                }
            }
        }
    }

    private void UpdateIslandsAndSleep(float dt)
    {
        int n = bodies.Count;
        if (islandParent.Length < n)
            islandParent = new int[Lib.Math.Power2Roundup(n)];

        for (int i = 0; i < n; i++)
        {
            bodies[i].islandIndex = i;
            islandParent[i] = i;
        }

        // Union bodies connected by a contact.
        // Static bodies never merge islands - otherwise the floor would
        // link every resting object into one island that can never sleep.
        for (int i = 0; i < manifolds.Count; i++)
        {
            Manifold m = manifolds[i];
            if (m.noImpulse) continue; //this is a trigger collision, and doesn't count for islands
            if (m.A.isStatic || m.B.isStatic) continue;
            UnionIslands(m.A.islandIndex, m.B.islandIndex);
        }

        // Pass 1: update each body's own idle timer from its own velocity.
        for (int i = 0; i < n; i++)
        {
            PhysicsBody b = bodies[i];
            if (!IsActive(b)) continue; //skip static and sleeping bodeies.

            if (!b.canSleep || b.forceAwakeThisStep)
            {
                b.sleepTime = 0f;
                continue;
            }

            float linSq = b.velocity.LengthSquared();
            float angSq = b.angularVelocity * b.angularVelocity;

            if (linSq > PhysicsBody.LinearSleepTolerance * PhysicsBody.LinearSleepTolerance ||
                angSq > PhysicsBody.AngularSleepTolerance * PhysicsBody.AngularSleepTolerance)
                b.sleepTime = 0f;
            else
                b.sleepTime += dt;
        }

        // Pass 2: aggregate per-island - the whole island's timer is the
        // MINIMUM across its members, and it's only sleep-eligible if every
        // member individually is (canSleep and not force-awake).
        islandMinSleepTime.Clear();
        islandCanSleep.Clear();

        for (int i = 0; i < n; i++)
        {
            PhysicsBody b = bodies[i];
            if (b.isStatic) continue;

            int root = FindIsland(i);
            bool eligible = b.canSleep && !b.forceAwakeThisStep;

            if (islandMinSleepTime.TryGetValue(root, out float existing))
            {
                islandMinSleepTime[root] = MathHelper.Min(existing, b.sleepTime);
                islandCanSleep[root] &= eligible;
            }
            else
            {
                islandMinSleepTime[root] = b.sleepTime;
                islandCanSleep[root] = eligible;
            }
        }

        // Pass 3: sleep or wake every body in the island together.
        for (int i = 0; i < n; i++)
        {
            PhysicsBody b = bodies[i];
            if (b.isStatic) continue;

            int root = FindIsland(i);
            bool canSleepNow = islandCanSleep[root] && islandMinSleepTime[root] >= PhysicsBody.TimeToSleep;

            if (canSleepNow)
                b.Sleep();
            else if (!b.IsAwake)
                b.WakeFromIsland();

            b.forceAwakeThisStep = false;
        }
    }

    private int FindIsland(int i)
    {
        while (islandParent[i] != i)
        {
            islandParent[i] = islandParent[islandParent[i]]; // path compression
            i = islandParent[i];
        }
        return i;
    }

    private void UnionIslands(int a, int b)
    {
        int ra = FindIsland(a);
        int rb = FindIsland(b);
        if (ra != rb)
            islandParent[ra] = rb;
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

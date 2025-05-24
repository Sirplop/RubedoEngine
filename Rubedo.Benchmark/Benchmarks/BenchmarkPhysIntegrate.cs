using BenchmarkDotNet.Attributes;
using Rubedo.Physics2D;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Constraints;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Dynamics.Shapes;
using System.Numerics;

namespace Rubedo.Benchmark.Benchmarks;

[MemoryDiagnoser]
public class BenchmarkPhysIntegrate
{
    Manifold m;
    public BenchmarkPhysIntegrate()
    {
        PhysicsMaterial mat = new PhysicsMaterial(1, 0.5f, 0.5f);
        PhysicsBody bodyA = new PhysicsBody(Collider.CreateUnitShape(ShapeType.Polygon, 3), mat, true, true);
        PhysicsBody bodyB = new PhysicsBody(Collider.CreateUnitShape(ShapeType.Polygon, 3), mat, true, true);

        m = new Manifold(bodyA, bodyB);
        Contact c = new Contact(Vector2.Zero);
        c.penetration = 0.25f;
        c.accumFriction = 50f;
        c.accumImpulse = 50f;
        Contact c2 = new Contact(Vector2.One);
        c2.penetration = 0.25f;
        c2.accumFriction = 50f;
        c2.accumImpulse = 50f;
        m.Update(c, c2);

        ContactConstraintSolver.PresolveConstraint(m, 1 / 60f);
        ContactConstraintSolver.WarmStart(m);
    }

    [Benchmark]
    public Manifold Integrate()
    {
        ContactConstraintSolver.ApplyImpulse(m);
        return m;
    }
}
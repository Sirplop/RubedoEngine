using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Xna.Framework;
using Rubedo.Object;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Dynamics.Shapes;
using Rubedo.Physics2D.Math;
using System.Collections.Generic;

namespace Rubedo.Benchmarks;

/// <summary>
/// I am BenchmarkCollisions, and this is my summary.
/// </summary>

public class BenchmarkCollisions
{
    Circle circle1;
    Circle circle2;
    Capsule capsule1;
    Capsule capsule2;
    Box box1;
    Box box2;
    Polygon poly1;
    Polygon poly2;

    Manifold manifold;

    public BenchmarkCollisions()
    {
        manifold = new Manifold(null, null);

        Transform onesTransform = new Transform(new Vector2(0.25f, 0.2f), 25f);
        Transform twosTransform = new Transform(new Vector2(-0.25f, -0.2f), -25f);
        Transform cap1Transform = new Transform(new Vector2(0.2f, 0), 0);
        Transform cap2Transform = new Transform(new Vector2(-0.2f, 0), 0);
        circle1 = new Circle(1f);
        circle2 = new Circle(1f);
        capsule1 = new Capsule(2f, 1f);
        capsule2 = new Capsule(2f, 1f);
        box1 = new Box(1f, 1f);
        box2 = new Box(1f, 1f);

        Collider p1 = Collider.CreateUnitShape(ShapeType.Polygon, false, 4);
        Collider p2 = Collider.CreateUnitShape(ShapeType.Polygon, false, 4);

        poly1 = new Polygon(((Polygon)p1.shape).vertices);
        poly2 = new Polygon(((Polygon)p2.shape).vertices);
    }

    //[Benchmark]
    public void BenchCircleCircle()
    {
        PhysicsCollisions.CircleToCircle(ref manifold, circle1, circle2);
    }
   // [Benchmark]
    public void BenchCircleCapsule()
    {
        PhysicsCollisions.CircleToCapsule(ref manifold, circle1, capsule2);
    }
    //[Benchmark]
    public void BenchCircleBox()
    {
        PhysicsCollisions.CircleToBox(ref manifold, circle1, box2);
    }
   // [Benchmark]
    public void BenchCirclePoly()
    {
        PhysicsCollisions.CircleToPolygon(ref manifold, circle1, poly2);
    }
   // [Benchmark]
    public void BenchCapsuleCapsule()
    {
        PhysicsCollisions.CapsuleToCapsule(ref manifold, capsule1, capsule2);
    }
    //[Benchmark]
    public void BenchCapsuleBox()
    {
        PhysicsCollisions.CapsuleToBox(ref manifold, capsule1, box2);
    }
 //   [Benchmark]
    public void BenchCapsulePoly()
    {
        PhysicsCollisions.CapsuleToPolygon(ref manifold, capsule1, poly2);
    }
    //[Benchmark]
    public void BenchBoxBox()
    {
        PhysicsCollisions.BoxToBox(ref manifold, box1, box2);
    }
    //[Benchmark]
    public void BenchBoxPoly()
    {
        PhysicsCollisions.BoxToPolygon(ref manifold, box1, poly2);
    }
    [Benchmark]
    public void BenchPolyPoly()
    {
        PhysicsCollisions.PolygonToPolygon(ref manifold, poly1, poly2);
    }
}
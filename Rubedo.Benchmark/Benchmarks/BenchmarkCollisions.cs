using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo.Object;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Dynamics;
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
        circle1 = new Circle(onesTransform, 1f);
        circle2 = new Circle(twosTransform, 1f);
        capsule1 = new Capsule(cap1Transform, 2f, 1f);
        capsule2 = new Capsule(cap2Transform, 2f, 1f);
        box1 = new Box(onesTransform, 1f, 1f);
        box2 = new Box(twosTransform, 1f, 1f);

        Collider p1 = Collider.CreateUnitShape(Physics2D.Collision.Shapes.ShapeType.Polygon, 4);
        Collider p2 = Collider.CreateUnitShape(Physics2D.Collision.Shapes.ShapeType.Polygon, 4);

        poly1 = new Polygon(onesTransform, ((Polygon)p1.shape).vertices);
        poly2 = new Polygon(twosTransform, ((Polygon)p2.shape).vertices);
    }

    //[Benchmark]
    public void BenchCircleCircle()
    {
        PhysicsCollisions.CircleToCircle(manifold, circle1, circle2);
    }
   // [Benchmark]
    public void BenchCircleCapsule()
    {
        PhysicsCollisions.CircleToCapsule(manifold, circle1, capsule2);
    }
    //[Benchmark]
    public void BenchCircleBox()
    {
        PhysicsCollisions.CircleToBox(manifold, circle1, box2);
    }
   // [Benchmark]
    public void BenchCirclePoly()
    {
        PhysicsCollisions.CircleToPolygon(manifold, circle1, poly2);
    }
   // [Benchmark]
    public void BenchCapsuleCapsule()
    {
        PhysicsCollisions.CapsuleToCapsule(manifold, capsule1, capsule2);
    }
    //[Benchmark]
    public void BenchCapsuleBox()
    {
        PhysicsCollisions.CapsuleToBox(manifold, capsule1, box2);
    }
 //   [Benchmark]
    public void BenchCapsulePoly()
    {
        PhysicsCollisions.CapsuleToPolygon(manifold, capsule1, poly2);
    }
    //[Benchmark]
    public void BenchBoxBox()
    {
        PhysicsCollisions.BoxToBox(manifold, box1, box2);
    }
    //[Benchmark]
    public void BenchBoxPoly()
    {
        PhysicsCollisions.BoxToPolygon(manifold, box1, poly2);
    }
    [Benchmark]
    public void BenchPolyPoly()
    {
        PhysicsCollisions.PolygonToPolygon(manifold, poly1, poly2);
    }
}
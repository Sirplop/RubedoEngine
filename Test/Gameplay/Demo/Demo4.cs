using Microsoft.Xna.Framework;
using Rubedo;
using Rubedo.Object;
using Rubedo.Physics2D;
using Rubedo.Physics2D.Dynamics;

namespace Test.Gameplay.Demo;

/// <summary>
/// Transform test
/// </summary>
internal class Demo4 : DemoBase
{
    private PhysicsBody polyBody;
    private PhysicsBody circleBody;

    private PhysicsBody meanPolyBody;
    private PhysicsBody meanCircleBody;
    private Collider meanCollider;
    private Vector2 circPos = new Vector2(5, 0);

    public Demo4()
    {
        description = "Transform Test";
    }

    public override void HandleInput(DemoState state) { }

    public override void Initialize(DemoState state)
    {
        Entity entity;
        Collider comp;
        PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f, 0, 0.5f);


        entity = new Entity(new Vector2(0, 0));
        comp = Collider.CreateUnitShape(Rubedo.Physics2D.Collision.Shapes.ShapeType.Polygon, 3);
        meanPolyBody = state.MakeBody(entity, material, comp, true);

        entity = new Entity(new Vector2(-5, -2.5f));
        meanCollider = Collider.CreateUnitShape(Rubedo.Physics2D.Collision.Shapes.ShapeType.Circle);
        meanCircleBody = state.MakeBody(entity, material, meanCollider, true);

        /*
        entity = new Entity(new Vector2(-5, 0));
        comp = Collider.CreateUnitShape(Rubedo.Physics2D.Collision.Shapes.ShapeType.Polygon, 3);
        polyBody = state.MakeBody(entity, material, comp, true);

        entity = new Entity(new Vector2(5, 0));
        comp = Collider.CreateUnitShape(Rubedo.Physics2D.Collision.Shapes.ShapeType.Circle);
        circleBody = state.MakeBody(entity, material, comp, true);

        circleBody.Entity.transform.SetParent(polyBody.transform);*/
    }

    public override void Update(DemoState state)
    {
        Vector2 curScale;
        float y;
        /*
        curScale = polyBody.Entity.transform.LocalScale;
        y = Rubedo.Lib.Wave.Sine((float)RubedoEngine.RawTime, 5000, 2, 0) + 3;
        curScale.Y = y;
        polyBody.Entity.transform.LocalScale = curScale;
        polyBody.Entity.transform.LocalRotation += RubedoEngine.DeltaTime;
        */
        curScale = meanPolyBody.Entity.transform.LocalScale;
        y = Rubedo.Lib.Wave.Sine((float)RubedoEngine.RawTime, 5000, 2, 0) + 3;
        curScale.Y = y;
        meanPolyBody.Entity.transform.LocalScale = curScale;
        meanPolyBody.Entity.transform.LocalRotation += RubedoEngine.DeltaTime;
        Vector2 pos = meanPolyBody.Entity.transform.WorldToLocalPosition(circPos);
        meanCircleBody.transform.LocalPosition = pos;
        meanCollider.transform.LocalPosition = pos;
    }
}
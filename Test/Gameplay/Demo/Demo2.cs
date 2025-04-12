using Rubedo;
using Rubedo.Object;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D;
using Microsoft.Xna.Framework;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Render;
using System.Diagnostics.Metrics;

namespace Test.Gameplay.Demo;

/// <summary>
/// Pillar test
/// </summary>
internal class Demo2 : DemoBase
{
    public Demo2()
    {
        description = "Pillar Test";
    }

    public override void Initialize(DemoState state)
    {
        RubedoEngine.Instance.Camera.GetExtents(out float width, out float height);
        PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f, 0, 0.5f);
        Entity entity;
        Collider comp;

        //left wall
        entity = new Entity(new Vector2(-width / 2 + 0.5f, 0.6f));
        comp = Collider.CreateBox(1, height - 0.5f);
        state.MakeBody(entity, material, comp, true);

        //right wall
        entity = new Entity(new Vector2(width / 2 - 0.5f, 0.6f));
        comp = Collider.CreateBox(1, height - 0.5f);
        state.MakeBody(entity, material, comp, true);

        //floor
        entity = new Entity(new Vector2(0, -height / 2 + 0.5f));
        comp = Collider.CreateBox(width, 1);
        state.MakeBody(entity, material, comp, true);

        float X = -width / 2 + 2f;
        float xChange = width / 11f - 0.1f;
        float Y = -height / 2 + 2f;
        float meter = 2;

        for (int i = 0; i < 11; i++)
        {
            for (int y = 0; y < 1 + (i * 1); y++)
            {
                entity = new Entity(new Vector2(X, Y + (meter * y)));
                comp = Collider.CreateBox(meter, meter);
                state.MakeBody(entity, material, comp, false);
            }
            X += xChange;
        }
    }
    public override void Update(DemoState state) { }

    private bool shapeSet = true;
    public override void HandleInput(DemoState state)
    {
        if (state.inputManager.MousePressed(InputManager.MouseButtons.Mouse1))
        {
            PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f, 0, 0.5f);

            //Circle circle = new Circle(0.5f);
            //shapes.MakeBody(circle, material, inputManager.MouseWorldPosition(), 45, false);
            Entity entity = new Entity(state.inputManager.MouseWorldPosition());
            Collider comp = Collider.CreateUnitShape(shapeSet ? ShapeType.Circle : ShapeType.Capsule);
            state.MakeBody(entity, material, comp, false);
        }
        if (state.inputManager.MousePressed(InputManager.MouseButtons.Mouse2))
        {
            PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f);
            //Polygon polygon = new Polygon(0.5f, 0.5f);
            //shapes.MakeBody(polygon, material, inputManager.MouseWorldPosition(), 45, false);
            Entity entity = new Entity(state.inputManager.MouseWorldPosition());
            Collider comp = Collider.CreateUnitShape(shapeSet ? ShapeType.Box : ShapeType.Polygon, 3);
            state.MakeBody(entity, material, comp, false);
        }
        if (state.inputManager.MousePressed(InputManager.MouseButtons.Mouse3))
        {
            shapeSet = !shapeSet;
        }
    }
}
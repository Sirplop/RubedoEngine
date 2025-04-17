using Rubedo.Object;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D;
using Rubedo;
using Microsoft.Xna.Framework;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Lib;

namespace Test.Gameplay.Demo;

/// <summary>
/// Pyramid Demo
/// </summary>
internal class Demo3 : DemoBase
{
    public Demo3()
    {
        description = "Pyramid";
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

        Vector2 pX = new Vector2(-width / 3, (-height / 2 + 1.5f));

        const int N = 19;

        for (int i = 0; i < N; ++i)
        {
            Vector2 y = pX;
            for (int j = i; j < N; ++j)
            {
                entity = new Entity(y);
                comp = Collider.CreateUnitShape(ShapeType.Box);
                state.MakeBody(entity, material, comp, false);

                y += Vector2.UnitX * 1.125f * Collider.UNIT_BOX_SIDE;
            }

            // x += Vector2(0.5625f, 1.125f);
            pX += new Vector2(0.5625f, 1.0f) * Collider.UNIT_BOX_SIDE;
        }
    }

    public override void Update(DemoState state) { }

    private bool shapeSet = true;
    public override void HandleInput(DemoState state)
    {
        if (state.inputManager.MousePressed(InputManager.MouseButtons.Mouse1) ||
            (DemoState.fastPlace && state.inputManager.MouseDown(InputManager.MouseButtons.Mouse1)))
        {
            PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f, 0, 0.5f);
            float x = Random.Range(0.5f, 2f);
            float y = Random.Range(0.5f, 2f);

            Entity entity = new Entity(state.inputManager.MouseWorldPosition(), 0, new Vector2(x, y));
            Collider comp = Collider.CreateUnitShape(shapeSet ? ShapeType.Circle : ShapeType.Capsule);
            state.MakeBody(entity, material, comp, false);
        }
        if (state.inputManager.MousePressed(InputManager.MouseButtons.Mouse2) ||
            (DemoState.fastPlace && state.inputManager.MouseDown(InputManager.MouseButtons.Mouse2)))
        {
            PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f);
            float x = Random.Range(0.5f, 2f);
            float y = Random.Range(0.5f, 2f);

            Entity entity = new Entity(state.inputManager.MouseWorldPosition(), 0, new Vector2(x, y));
            Collider comp = Collider.CreateUnitShape(shapeSet ? ShapeType.Box : ShapeType.Polygon, 3);
            state.MakeBody(entity, material, comp, false);
        }
        if (state.inputManager.MousePressed(InputManager.MouseButtons.Mouse3))
        {
            shapeSet = !shapeSet;
        }
    }
}
using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Render;

namespace Test.Gameplay.Demo;

/// <summary>
/// Body Sandbox
/// </summary>
internal class Demo1 : DemoBase
{
    private bool layout = false;

    private Entity orb;
    private Entity poly;
    private Entity capsule;

    private Entity ramp1;
    private Entity ramp2;

    public Demo1()
    {
        description = "Body Sandbox";
    }

    public override void Initialize(DemoState state)
    {
        RubedoEngine.Instance.Camera.GetExtents(out float width, out float height);

        PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0, 0, 0.5f);
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

        Layout(state);
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

        if (state.inputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
        {
            layout = !layout;
            Layout(state);
        }
    }

    private void Layout(DemoState state)
    {
        RubedoEngine.Instance.Camera.GetExtents(out float width, out float height);
        PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0, 0, 0.5f);
        Collider comp;

        if (!layout)
        {
            if (ramp1 != null)
                state.Remove(ramp1);
            if (ramp2 != null)
                state.Remove(ramp2);
            ramp1 = ramp2 = null;

            //magnificent orb
            orb = new Entity(new Vector2(-width / 4, 0));
            comp = Collider.CreateCircle(width * 0.1f);
            state.MakeBody(orb, material, comp, true);

            //magnificent polygon
            poly = new Entity(new Vector2(0, 0), 45, new Vector2(5f, 10f));
            comp = Collider.CreateUnitShape(ShapeType.Polygon, 3);
            state.MakeBody(poly, material, comp, true);

            //magnificent capsule
            capsule = new Entity(new Vector2(width / 4, 0), -0, new Vector2(1, 2));
            comp = Collider.CreateCapsule(width * 0.1f, width * 0.1f);
            state.MakeBody(capsule, material, comp, true);
        }
        else
        {
            if (orb != null)
                state.Remove(orb);
            if (poly != null)
                state.Remove(poly);
            if (capsule != null)
                state.Remove(capsule);
            orb = poly = capsule = null;

            //ramp 1
            ramp1 = new Entity(new Vector2(-width / 4, -height / 4), -102.25f);
            comp = Collider.CreateCapsule(width / 2, 1);
            state.MakeBody(ramp1, material, comp, true);
            //ramp 2
            ramp2 = new Entity(new Vector2(width / 4, height / 4), 102.25f);
            comp = Collider.CreateCapsule(width / 2, 1);
            state.MakeBody(ramp2, material, comp, true);
        }
    }
}